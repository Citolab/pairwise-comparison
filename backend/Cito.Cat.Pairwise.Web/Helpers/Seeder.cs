using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cito.Cat.Algorithms.Pairwise.Models;
using Cito.Cat.Core.Helpers;
using Cito.Cat.Core.Models.Section;
using Cito.Cat.Pairwise.Web.Models;
using Cito.Cat.Service.Handlers;
using ExcelDataReader;
using Ims.Cat.Models;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    public class Seeder
    {
        private readonly IAsyncDocumentSession _documentSession;
        private readonly CatSectionHandler _catSectionHandler;
        private readonly string _contentRootPath;
        private readonly ILogger<Seeder> _logger;

        public Seeder(IAsyncDocumentSession documentSession, CatSectionHandler catSectionHandler,
            string contentRootPath, ILoggerFactory loggerFactory)
        {
            _documentSession = documentSession;
            _catSectionHandler = catSectionHandler;
            _contentRootPath = contentRootPath;
            _logger = loggerFactory.CreateLogger<Seeder>();
        }

        public async Task SeedCatSectionsV2()
        {
            if (!await _documentSession.Query<CatSection>().AnyAsync()) // don't seed if there's already data.
            {
                await SeedPilotSet();

                // small sample set (6 texts)
                await SeedSmallSampleSet();

                // larger sample set 
                await SeedLargeSampleSet();

                await _documentSession.SaveChangesAsync();
                _logger.LogInformation("Done seeding");
            }
        }

        private async Task SeedLargeSampleSet()
        {
            var largerSampleSetPath = Path.Combine(_contentRootPath, "SeedData/Teksten_Cito");
            if (!Directory.Exists(largerSampleSetPath))
            {
                return;
            }

            var itemsSampleLarger = GetTexts(Path.Combine(_contentRootPath, "SeedData/Teksten_Cito"));
            if (!itemsSampleLarger.Any())
            {
                return;
            }

            var sectionConfigSampleLarger = new SectionConfiguration
            {
                Title = "Sample section (larger)",
                SamplingIterations = 1,
                ComparisonsPerItem = 3,
                // total number of comparisons:  (int) Math.Round((_sectionConfig.ItemCount * _sectionConfig.ComparisonsPerItem) / 2.0),
                ComparisonsTotal = (int) Math.Round((itemsSampleLarger.Count * 3) / 2.0),
                ItemIds = itemsSampleLarger.Keys.ToArray(),
                ThetaStart = Enumerable.Repeat(0d, itemsSampleLarger.Count).ToArray()
            };
            var largerSampleSectionConfigBase64 = sectionConfigSampleLarger.ToJson().Base64Encode();
            var largerSampleInitResponse = await _catSectionHandler.Initialize(new SectionDType
            {
                SectionConfiguration = largerSampleSectionConfigBase64
            });

            var sectionSampleLarger = new TestSection
            {
                Title = sectionConfigSampleLarger.Title,
                CatSectionId = largerSampleInitResponse.SectionIdentifier,
                Items = itemsSampleLarger
            };
            await _documentSession.StoreAsync(sectionSampleLarger);
            var sessionSampleLarger = new TestSession
            {
                Name = "Assessor 1",
                TestSectionIdentifier = sectionSampleLarger.Id,
                CatSectionIdentifier = largerSampleInitResponse.SectionIdentifier,
                Status = TestStatus.NotStarted,
                StartCode = "1234"
            };
            await _documentSession.StoreAsync(sessionSampleLarger);
        }

        private async Task SeedSmallSampleSet()
        {
            var itemsSampleSmall = GetSmallSetTexts();
            var sectionConfigSampleSmall = new SectionConfiguration
            {
                Title = "Sample section (6 texts)",
                SamplingIterations = 1,
                ComparisonsPerItem = 3,
                // total number of comparisons:  (int) Math.Round((_sectionConfig.ItemCount * _sectionConfig.ComparisonsPerItem) / 2.0),
                ComparisonsTotal = (int) Math.Round((itemsSampleSmall.Count * 3) / 2.0),
                ItemIds = itemsSampleSmall.Keys.ToArray(),
                ThetaStart = Enumerable.Repeat(0d, itemsSampleSmall.Count).ToArray()
            };
            var smallSampleSectionConfigBase64 = sectionConfigSampleSmall.ToJson().Base64Encode();
            var smallSampleInitResponse = await _catSectionHandler.Initialize(new SectionDType
            {
                SectionConfiguration = smallSampleSectionConfigBase64
            });
            var sectionSampleSmall = new TestSection
            {
                Title = sectionConfigSampleSmall.Title,
                CatSectionId = smallSampleInitResponse.SectionIdentifier,
                Items = itemsSampleSmall
            };
            await _documentSession.StoreAsync(sectionSampleSmall);
            var sessionSampleSmall = new TestSession
            {
                Name = "Assessor 1",
                TestSectionIdentifier = sectionSampleSmall.Id,
                CatSectionIdentifier = smallSampleInitResponse.SectionIdentifier,
                Status = TestStatus.NotStarted,
                StartCode = "0000"
            };
            await _documentSession.StoreAsync(sessionSampleSmall);
        }

        private async Task SeedPilotSet()
        {
            var pilotSetPath = Path.Combine(_contentRootPath, "SeedData/Teksten_2021-02-04");
            if (!Directory.Exists(pilotSetPath))
            {
                return;
            }

            var (items, expertToItems) =
                await GetTextsFromExcelV2(pilotSetPath);

            if (items == null || expertToItems == null)
            {
                return;
            }

            foreach (var (expert, itemIds) in expertToItems)
            {
                var sectionConfig = new SectionConfiguration
                {
                    Title = $"Set for {expert}",
                    SamplingIterations = 1,
                    ComparisonsPerItem = 2,
                    ComparisonsTotal = (int) Math.Round((itemIds.Count * 2) / 2.0),
                    ItemIds = itemIds.ToArray(),
                    ThetaStart = Enumerable.Repeat(0d, itemIds.Count).ToArray()
                };
                var sectionConfigB64 = sectionConfig.ToJson().Base64Encode();
                var res = await _catSectionHandler.Initialize(new SectionDType
                {
                    SectionConfiguration = sectionConfigB64
                });

                var section = new TestSection
                {
                    Title = sectionConfig.Title,
                    CatSectionId = res.SectionIdentifier,
                    Items = items.Where(pair => itemIds.Contains(pair.Key)).ToDictionary(x => x.Key, x => x.Value)
                };
                await _documentSession.StoreAsync(section);
                var session = new TestSession
                {
                    Name = expert,
                    TestSectionIdentifier = section.Id,
                    CatSectionIdentifier = res.SectionIdentifier,
                    Status = TestStatus.NotStarted,
                    StartCode = expert
                };
                await _documentSession.StoreAsync(session);
            }
        }

        private async Task<(Dictionary<string, Item> items, Dictionary<string, List<string>> expertToItems)>
            GetTextsFromExcelV2(string baseDir)
        {
            var dirInfo = new DirectoryInfo(baseDir);
            var excelFiles = dirInfo.GetFiles("*.xlsx").Where(f => !f.Name.StartsWith("~$")).ToArray();
            if (excelFiles.Length != 1) // allow one and only 1 excel file
            {
                return (null, null);
            }

            await using var stream = excelFiles[0].OpenRead();
            using var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
            {
                FallbackEncoding = Encoding.UTF8
            });
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = dataReader => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            var sheet = dataSet.Tables["Teksten"];

            if (!CheckColumnNamesV2(sheet))
            {
                return (null, null);
            }

            var items = new Dictionary<string, Item>();
            var expertToItems = new Dictionary<string, List<string>>();
            var filesNotFound = new StringBuilder();
            var filesNotFoundCount = 0;
            var shortTextItems = new Dictionary<string, Item>();
            foreach (DataRow row in sheet.Rows)
            {
                var id = row["id"].ToString(); //0
                var item = new Item();
                item.Id = id;
                item.FileName = row["Bestandsnaam"] as string; //1  
                item.FileName = item.FileName?.Trim();
                item.Category = row["Type tekst"] as string; //2
                //item.EstimatedReadingLevel = row["Geschat leesniveau"] as string; //3
                item.Source = row["Bron"] as string; //3
                //item.Publicatiedatum = row["Publicatiedatum"] as string; //4
                item.License = row["Licentie"] as string; //5
                item.MeijerinkOrigineel = (int) (double) row["Meijerink origineel"]; //6
                item.LeesindexAOrigineel = (int) (double) row["leesindex A origineel"]; //7
                item.MeijerinkIngekort = (int) (double) row["Meijerink ingekort"]; //8
                item.LeesindexAIngekort = (int) (double) row["leesindex A ingekort"]; //9

                // load the text
                var filePath = Path.Combine(baseDir, "Teksten", item.Category, item.FileName);
                if (!File.Exists(filePath))
                {
                    filesNotFound.Append($"{item.Id} '{filePath}' not found." + Environment.NewLine);
                    filesNotFoundCount++;
                    continue;
                }

                item.Text = await File.ReadAllTextAsync(filePath);
                if (item.Text.GetWordCount() <= 100)
                {
                    shortTextItems.Add(id, item);
                }
                else
                {
                    items.Add(id, item);
                    for (var i = 10; i < sheet.Columns.Count; i++)
                    {
                        var expert = sheet.Columns[i].ColumnName;
                        if ((double) row[i] > 0)
                        {
                            if (!expertToItems.ContainsKey(expert))
                            {
                                expertToItems[expert] = new List<string>();
                            }

                            expertToItems[expert].Add(id);
                        }
                    }
                }
            }

            if (filesNotFoundCount > 0)
            {
                var errors = filesNotFound.ToString();
                _logger.LogError($"{errors}{filesNotFoundCount} files not found.");
                return (null, null);
            }

            if (shortTextItems.Any())
            {
                var shortTexts = string.Join(Environment.NewLine,
                    shortTextItems.Select(i =>
                        $"Id:{i.Key}: #words: {i.Value.Text.GetWordCount()}, {i.Value.Category}, {i.Value.FileName}"));
                _logger.LogWarning("{Count} Short texts found and ignored:\n{ShortTexts}", shortTextItems.Count,
                    shortTexts);
            }

            return (items, expertToItems);
        }

        private static bool CheckColumnNamesV2(DataTable sheet)
        {
            if (sheet.Columns.Count < 11)
            {
                return false;
            }

            bool result;
            result = sheet.Columns[0].ColumnName == "id";
            result &= sheet.Columns[1].ColumnName == "Bestandsnaam";
            result &= sheet.Columns[2].ColumnName == "Type tekst";
            result &= sheet.Columns[3].ColumnName == "Bron";
            result &= sheet.Columns[4].ColumnName == "Publicatiedatum";
            result &= sheet.Columns[5].ColumnName == "Licentie";
            result &= sheet.Columns[6].ColumnName == "Meijerink origineel";
            result &= sheet.Columns[7].ColumnName == "leesindex A origineel";
            result &= sheet.Columns[8].ColumnName == "Meijerink ingekort";
            result &= sheet.Columns[9].ColumnName == "leesindex A ingekort";

            return result;
        }

        private static Dictionary<string, Item> GetTexts(string baseDir)
        {
            var dirInfo = new DirectoryInfo(baseDir);
            var files = dirInfo.GetFiles("*.txt", SearchOption.AllDirectories);
            return files.Select(f =>
                    (ItemId: Path.GetFileNameWithoutExtension(f.Name),
                        Item: new Item(Path.GetFileNameWithoutExtension(f.Name), File.ReadAllText(f.FullName))))
                .ToList()
                .ToDictionary(i => i.ItemId, i => i.Item);
        }

        private static Dictionary<string, Item> GetSmallSetTexts()
        {
            var result = new Dictionary<string, Item>
            {
                {
                    "A2+_item01", new Item(
                        @"When Winnie the Witch turned over the page on her calendar, she saw a big red circle around Friday the thirteenth.
'That's my birthday!' she said. 'I'll have a party this year, Wilbur.' 'Purr,' said Wilbur. He loved parties.
'What kind of party?' Winnie wondered. 'I know, a garden party.'
On Monday Winnie wrote out the invitations and sent them by Winni-e-mail. She invited . . .
Aunty Alice, Uncle Owen,
her three sisters Wanda, Wilma and Wendy, all of her friends, and Cousin Cuthbert.
On Tuesday she made herself a party dress,
and a matching bow for Wilbur.
'Purr,' said Wilbur. I look lovely, he thought

On Wednesday Winnie made lots and lots of food Wilbur helped.



Thursday was the day to get the garden ready. Winnie went outside. It looked rather scruffy Then Winnie had a very good idea. She took out her wand, waved it, shouted,
And the garden was ready for the party 'That was easy,' Winnie said.
'Now what else? Oh yes, I need a surprise A good party always has a surprise. I'll have to think about that.'
Friday the thirteenth was a lovely sunny day, which was lucky.
At two o'clock Winnie's guests arrived. 'Happy birthday, Winnie,' they shouted, and they piled up the presents on the lawn
Wanda, Wilma and Wendy gave Winnie a magic carpet. She'd always wanted one of those.
Uncle Owen gave her a bat in a cage. She'd never wanted one of those.
Aunty Alice gave her a Book of Special Spells,")
                },
                {
                    "A2+_item02", new Item(@"You can have lots of fun with ice and snow.
Lots of children like ice skating.
You can slide on ice at a playground. Or you can skate at an ice rink.
Take three strides - and then glide along the ice!
Add Jive stone buttons and a carrot nose.
Add a mouth made from small stones to make the snowman smile.
Let the snowman borrow a nice bright scarf!
Pile up lots of snow to make a snowman.
See who can make the biggest pile of snow in Jive minutes.
Or start a snowball fight!
Get into two teams and see which team can score more hits.
Ride a toboggan down a snowy Kill.
Toboggans can go quite fast.
Set up a toboggan race.
You all line up at the top of the hill - the first to reach the bottom is the winner.
Set up a snowball and spoon race.
You must keep the snowball on your spoon as you race for the finish.
Or look for animal footprints in the snow.
bird
dog
The size and shape of the prints will tell you which animal made them.
squirrel
cat
horse
fox
Make a snow angel.
Lie on your back in the snow and swing your arms up and down to make wings.
When you stand up, you will see the snow angel.")
                },
                {
                    "A2+_item03", new Item(@"CHAPTER ONE



What Happened to Holly Golightly?



I sometimes visit places where I lived in the past - the houses and their neighborhoods. I like to see them again. There's a brown stone house in the East Seventies where, during the early years of the war, I had my first New York apartment. It was one room, crowded with an old red sofa and red chairs. The walls were dark and dirty from old cigarette smoke. The single window looked out onto a fire escape, a stairway that went down to the street. It wasn't a big place but it made me happy. It was my first home, and my books were there, and a box of pencils. Everything that a writer needed, I thought.



I didn't write about Holly Golightly in those days. I'm only writing about her now because of a conversation that I had with Joe Bell.

Holly Golightly was another tenant in the old brown stone house, in the apartment below mine. Joe Bell had a bar around the corner; he's still there. Both Holly and I went there six or seven times every day, not for a drink - not always - but to make telephone calls. During the war few people had a private telephone. Joe Bell took messages for us. Holly got a lot of messages.

Of course, this was a long time ago. I didn't see Joe Bell for years, not until last week. We weren't close friends but we were both friends of Holly Golightly.")
                },
                {
                    "B2_item01", new Item(@"Keeping the holiday-makers happy

A chalet girl’s work is never done, Sarah Sutherland-Pilch tells Veronica Lee - in between making beds and delicious dinners. 

This is the second year as a chalet girl for Sarah Sutherland-Pilch, a 24-year-old from West Sussex. Known by her nickname, Pilch, Sarah works for a company in Val d’Isere, France, cooking and cleaning for visitors who come to ski and stay in the wooden houses, known as chalets, that are characteristic of the area. Sarah graduated in French and History of Art from Oxford Brookes University last summer. 
Being a chalet girl isn't a career, she says, but an enjoyable way to spend a year or two before settling down. 'It's a good way to make contacts. I meet successful people every week.' 
Sarah does not 'live in'. She gets up at 7a.m. to walk the mile or so to the chalet, which sleeps up to 18 guests each week. She has her own breakfast before preparing that of the guests. 'They get the works - porridge, eggs, cereals, fruit and croissants.' When the last of the guests has had breakfast, by about 9.30 a.m., Sarah clears up and either makes the afternoon tea, which is left for the guests to help themselves, or cleans the rooms - 'the worst part of the job,' she says. 
By about 11 a.m. she is ready to go on the slopes herself. She skis as much as possible. ‘On a good day we can be up there until 4.30 p.m.' Sarah returns to the chalet in time to prepare dinner and takes a shower before doing so, but does not sleep. 'It's fatal if you do,' she says. 
Dinner, a three-course affair, is served at 8 p.m. and coffee is usually on the table by 10 p.m. Sarah clears away the dinner things and fills the dishwasher. 'There's nothing worse than coming in to a messy kitchen the next morning.' Sometimes she will stay and chat with the guests, other times they are content to be left alone. 'Good guests can make a week brilliant - breakfast this morning was great fun - but some weeks, for whatever reason, don't go quite so well.' 
Sarah meets her friends in the chalet where she lives - and they go out at about 11 p.m. 'We usually start off in Bananas, might go to G Jay’s and perhaps Dick’s T-Bar at the end of the evening,' she says. But Sarah never stays out too late on Saturday night as Sunday is her busiest time of the week. ‘A frightful day,' she says, 'when you certainly don't want to be cooking breakfast with a terrible headache.' 
Work begins earlier than usual on Sunday, since breakfast for guests who are leaving has to be on the table by 7 a.m. As soon as the guests are gone, Sarah starts cleaning madly. ’We just blitz the place - clear the breakfast, strip the beds, get everything ready.' If she hasn't already done the week's shop on Saturday, Sarah does it now. 
At around 3 p.m., the cleaning work done, Sarah then prepares tea for the new guests. 'They get here at around 4.30 p.m.' Sometimes they are disorientated and full of questions. I'm sure it's the mountain air that does something to them.' 
Between tea and dinner, Sarah takes any guests needing boots or skis down to the ski shop and then gets a lift back to the chalet from one of the ski shop staff. It is soon time for dinner duty again and perhaps a drink later, but not always. ’Sometimes I'm so tired I just have an early night,' she says. ")
                },
                {
                    "B2_item02", new Item(@"	Part one

	Chapter one

	'Don't you agree that she's got to be killed?' The words seemed to hang in the still night air, before disappearing into the darkness.

	It was Hercule Poirot's first night in the city of Jerusalem, and he was shutting his hotel-room window - the night air was a danger to his health! - when he overheard these words. He smiled. 'Even on holiday, I am reminded of crime,' he said to himself. 'No doubt someone is talking about a play or a book.' As he walked over to his bed, he thought about the voice he had heard. It was the voice of a man - or a boy - and had sounded nervous and excited. 'I will remember that voice,' said Hercule Poirot to himself, as he lay down to sleep. 'Yes, I will remember.'

	In the room next door, Raymond Boynton and his sister Carol looked out of their window into the dark-blue night sky. Raymond said again, 'Don't you agree that she's got to be killed? It can't go on like this - it can't. We must do something - and what else can we do?'

	Carol said in a hopeless voice, 'If only we could just leave somehow! But we can't - we can't.'

	'People would say we were crazy,' said Raymond bitterly. 'They would wonder why we can't just walk out -'

	Carol said slowly, 'Perhaps we are crazy! '

	'Perhaps we are,' agreed Raymond. 'After all, we are calmly planning to kill our own mother! '")
                },
                {
                    "B2_item03", new Item(@"The Netball Captain

In our series on women in sport, Suzie Ellis went to meet England’s netball captain.

Kendra Slawinski is captain of England's netball team. When I met her, she'd had a typical day for the weeks leading up to next month's World Championships: a day's teaching at a local school followed by a training session in the local supermarket car park.
I was surprised to hear about her training venue.
'Don't you get strange looks?' I asked her. 'I'm too involved in what I'm doing concentrating on my movements and my feet - to see anything else,' she said.'I might notice cars slow down out of the corner of my eye, but that's all.'
'My whole life now is all about making sure I'm at my absolute best for the Championships,' says Kendra. I'm on a strict timetable to gain maximum fitness for them. These are her fourth World Championships and they are guaranteed to be the biggest ever, with 27 nations taking part.
'We'll have home support behind us, which is so special,' she says. 'And it's important that the reputation of netball in this country should be improved. As a result of playing here, there will be more pressure than we're used to. A home crowd will have expectations and give more support. People will expect us to start the tournament with a good game.'
Their first game is against Barbados and it comes immediately after the opening ceremony. As far as I'm aware, we have always beaten them, but they'll be exciting to play. They have lots of ability.'
The England team are currently ranked fourth in the world. But, as Kendra points out, the World Championships will be tough. 'You have to push yourself to play each day, there's no rest between games as in a series. And you can still win an international series if you lose the first game. But the Championships are different because there's only one chance and you have to be ready to make the most of it.
In the fifteen years since she has been playing at top level, the sport has become harder, faster. On court, players are more aggressive. 'You don't do all that training not to come out a winner,' says Kendra.But once the final whistle blows, you become a different person. We’re all friendlier after the game.' 
Netball is also taking a far more scientific approach to fitness testing.
'It is essential that we all think and train like world-class players,' says Kendra.
As captain, I think it's important that I have a strong mental attitude and lead by example. I see my role as supporting and encouraging the rest of the team.'
'From the very beginning, my netball career has always been carefully planned,' she says. So I took the decision some time ago that this competition would be the end of it as far as playing is concerned.
Doubtless she will coach young players in the future, but at the moment her eyes are firmly set on her last big event. As she leads out her team in the opening candlelight ceremony, she is more than likely to have a tear in her eye. Her loyal supporters will be behind her every step of the way.")
                }
            };

            foreach (var (id, item) in result)
            {
                item.Id = id;
            }

            return result;
        }
    }
}