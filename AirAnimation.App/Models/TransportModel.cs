namespace AirAnimation.App.Models;

public sealed class TransportModel
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Emoji { get; init; } = string.Empty;
    public string SvgIcon { get; init; } = string.Empty; // SVG path content
    public double DefaultSpeed { get; init; } = 80; // km/h for route time estimation
    public bool FollowRoads { get; init; } = true;
    public double RotationOffset { get; init; } = 0; // degrees adjustment for icon facing
    public string Description { get; init; } = string.Empty;

    public static IReadOnlyList<TransportModel> All => _all;

    private static readonly List<TransportModel> _all = BuildAll();

    private static List<TransportModel> BuildAll() =>
    [
        // ─── AUTO (MODERN & PREMIUM) ────────────────────────────────
        new() { Id="tesla_s_plaid",  Name="Tesla Model S",       Category="Авто", Emoji="🚗", DefaultSpeed=160, FollowRoads=true, SvgIcon=SvgIcons.SportsCar },
        new() { Id="tesla_cybertruck",Name="Tesla Cybertruck",   Category="Авто", Emoji="🛻", DefaultSpeed=140, FollowRoads=true, SvgIcon=SvgIcons.Pickup },
        new() { Id="porsche_taycan", Name="Porsche Taycan",      Category="Авто", Emoji="🏎️", DefaultSpeed=170, FollowRoads=true, SvgIcon=SvgIcons.SportsCar },
        new() { Id="lucid_air",      Name="Lucid Air",           Category="Авто", Emoji="🚗", DefaultSpeed=150, FollowRoads=true, SvgIcon=SvgIcons.Car },
        new() { Id="hyundai_ioniq6", Name="Hyundai Ioniq 6",     Category="Авто", Emoji="🚘", DefaultSpeed=130, FollowRoads=true, SvgIcon=SvgIcons.Car },
        
        new() { Id="rivian_r1s",     Name="Rivian R1S",          Category="Авто", Emoji="🚙", DefaultSpeed=120, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="volvo_ex90",     Name="Volvo EX90",          Category="Авто", Emoji="🚙", DefaultSpeed=110, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="bmw_ix",         Name="BMW iX",              Category="Авто", Emoji="🚙", DefaultSpeed=130, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="genesis_gv60",   Name="Genesis GV60",        Category="Авто", Emoji="🚙", DefaultSpeed=120, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="defender_110",   Name="Defender 110",        Category="Авто", Emoji="🚙", DefaultSpeed=110, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        
        new() { Id="audi_etron_gt",  Name="Audi e-tron GT",      Category="Авто", Emoji="🏎️", DefaultSpeed=160, FollowRoads=true, SvgIcon=SvgIcons.SportsCar },
        new() { Id="corvette_eray",  Name="Corvette E-Ray",      Category="Авто", Emoji="🏎️", DefaultSpeed=180, FollowRoads=true, SvgIcon=SvgIcons.SportsCar },
        new() { Id="mustang_mache",  Name="Mustang Mach-E",      Category="Авто", Emoji="🚙", DefaultSpeed=140, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="zeekr_001",      Name="Zeekr 001",           Category="Авто", Emoji="🏎️", DefaultSpeed=150, FollowRoads=true, SvgIcon=SvgIcons.SportsCar },
        new() { Id="lotus_eletre",   Name="Lotus Eletre",        Category="Авто", Emoji="🚙", DefaultSpeed=160, FollowRoads=true, SvgIcon=SvgIcons.Suv },

        new() { Id="toyota_prius",   Name="Toyota Prius",        Category="Авто", Emoji="🚗", DefaultSpeed=110, FollowRoads=true, SvgIcon=SvgIcons.Car },
        new() { Id="vw_idbuzz",      Name="VW ID. Buzz",         Category="Авто", Emoji="🚐", DefaultSpeed=100, FollowRoads=true, SvgIcon=SvgIcons.Van },
        new() { Id="kia_ev9",        Name="Kia EV9",             Category="Авто", Emoji="🚙", DefaultSpeed=120, FollowRoads=true, SvgIcon=SvgIcons.Suv },
        new() { Id="mini_cooper_se", Name="Mini Cooper SE",      Category="Авто", Emoji="🚘", DefaultSpeed=100, FollowRoads=true, SvgIcon=SvgIcons.Car },
        new() { Id="polestar_2",     Name="Polestar 2",          Category="Авто", Emoji="🚗", DefaultSpeed=130, FollowRoads=true, SvgIcon=SvgIcons.Car },

        // Utility Auto
        new() { Id="bus",        Name="Автобус",             Category="Авто",     Emoji="🚌",  DefaultSpeed=70,  FollowRoads=true,  SvgIcon=SvgIcons.Bus },
        new() { Id="truck",      Name="Грузовик",            Category="Авто",     Emoji="🚛",  DefaultSpeed=75,  FollowRoads=true,  SvgIcon=SvgIcons.Truck },
        new() { Id="tractor",    Name="Трактор",             Category="Авто",     Emoji="🚜",  DefaultSpeed=30,  FollowRoads=true,  SvgIcon=SvgIcons.Tractor },

        // ─── AVIATION ───────────────────────────────────────────────
        new() { Id="airliner",   Name="Авиалайнер",          Category="Авиация",  Emoji="✈️", DefaultSpeed=900, FollowRoads=false, SvgIcon=SvgIcons.Airliner },
        new() { Id="bizjet",     Name="Бизнес-джет",         Category="Авиация",  Emoji="🛩️",DefaultSpeed=800, FollowRoads=false, SvgIcon=SvgIcons.BizJet },
        new() { Id="helicopter", Name="Вертолёт",            Category="Авиация",  Emoji="🚁",  DefaultSpeed=250, FollowRoads=false, SvgIcon=SvgIcons.Helicopter },
        new() { Id="drone",      Name="Дрон",                Category="Авиация",  Emoji="🛸",  DefaultSpeed=60,  FollowRoads=false, SvgIcon=SvgIcons.Drone },
        new() { Id="glider",     Name="Планёр",              Category="Авиация",  Emoji="🛩️",DefaultSpeed=150, FollowRoads=false, SvgIcon=SvgIcons.Glider },
        new() { Id="airship",    Name="Дирижабль",           Category="Авиация",  Emoji="🚀",  DefaultSpeed=100, FollowRoads=false, SvgIcon=SvgIcons.Airship },
        new() { Id="hot_balloon",Name="Воздушный шар",       Category="Авиация",  Emoji="🎈",  DefaultSpeed=20,  FollowRoads=false, SvgIcon=SvgIcons.HotBalloon },
        new() { Id="fighter",    Name="Истребитель",         Category="Авиация",  Emoji="✈️", DefaultSpeed=1800,FollowRoads=false, SvgIcon=SvgIcons.Fighter },
        new() { Id="seaplane",   Name="Гидросамолёт",        Category="Авиация",  Emoji="✈️", DefaultSpeed=300, FollowRoads=false, SvgIcon=SvgIcons.Seaplane },

        // ─── SPACE ──────────────────────────────────────────────────
        new() { Id="rocket",     Name="Ракета",              Category="Космос",   Emoji="🚀",  DefaultSpeed=7000,FollowRoads=false, SvgIcon=SvgIcons.Rocket },
        new() { Id="shuttle",    Name="Космический шаттл",   Category="Космос",   Emoji="🚀",  DefaultSpeed=7700,FollowRoads=false, SvgIcon=SvgIcons.Shuttle },
        new() { Id="satellite",  Name="Спутник",             Category="Космос",   Emoji="🛰️",DefaultSpeed=7000,FollowRoads=false, SvgIcon=SvgIcons.Satellite },

        // ─── SEA ────────────────────────────────────────────────────
        new() { Id="yacht",      Name="Яхта",                Category="Море",     Emoji="⛵",  DefaultSpeed=15,  FollowRoads=false, SvgIcon=SvgIcons.Yacht },
        new() { Id="cruise",     Name="Круизный лайнер",     Category="Море",     Emoji="🚢",  DefaultSpeed=40,  FollowRoads=false, SvgIcon=SvgIcons.CruiseShip },
        new() { Id="ferry",      Name="Паром",               Category="Море",     Emoji="⛴️", DefaultSpeed=30,  FollowRoads=false, SvgIcon=SvgIcons.Ferry },
        new() { Id="submarine",  Name="Подводная лодка",     Category="Море",     Emoji="🚢",  DefaultSpeed=35,  FollowRoads=false, SvgIcon=SvgIcons.Submarine },
        new() { Id="motorboat",  Name="Моторная лодка",      Category="Море",     Emoji="🚤",  DefaultSpeed=60,  FollowRoads=false, SvgIcon=SvgIcons.Motorboat },
        new() { Id="kayak",      Name="Каяк",                Category="Море",     Emoji="🛶",  DefaultSpeed=8,   FollowRoads=false, SvgIcon=SvgIcons.Kayak },

        // ─── RAIL ───────────────────────────────────────────────────
        new() { Id="train",      Name="Поезд",               Category="Ж/д",      Emoji="🚆",  DefaultSpeed=120, FollowRoads=false, SvgIcon=SvgIcons.Train },
        new() { Id="tgv",        Name="Скоростной поезд",    Category="Ж/д",      Emoji="🚄",  DefaultSpeed=320, FollowRoads=false, SvgIcon=SvgIcons.TGV },
        new() { Id="metro",      Name="Метро",               Category="Ж/д",      Emoji="🚇",  DefaultSpeed=80,  FollowRoads=false, SvgIcon=SvgIcons.Metro },
        new() { Id="tram",       Name="Трамвай",             Category="Ж/д",      Emoji="🚊",  DefaultSpeed=50,  FollowRoads=true,  SvgIcon=SvgIcons.Tram },
        new() { Id="monorail",   Name="Монорельс",           Category="Ж/д",      Emoji="🚝",  DefaultSpeed=100, FollowRoads=false, SvgIcon=SvgIcons.Monorail },

        // ─── MOTO / MICRO ───────────────────────────────────────────
        new() { Id="motorcycle", Name="Мотоцикл",            Category="Мото",     Emoji="🏍️", DefaultSpeed=120, FollowRoads=true,  SvgIcon=SvgIcons.Motorcycle },
        new() { Id="scooter",    Name="Скутер",              Category="Мото",     Emoji="🛵",  DefaultSpeed=60,  FollowRoads=true,  SvgIcon=SvgIcons.Scooter },
        new() { Id="bicycle",    Name="Велосипед",           Category="Мото",     Emoji="🚴",  DefaultSpeed=25,  FollowRoads=true,  SvgIcon=SvgIcons.Bicycle },
        new() { Id="escooter",   Name="Электросамокат",      Category="Мото",     Emoji="🛴",  DefaultSpeed=25,  FollowRoads=true,  SvgIcon=SvgIcons.EScooter },
        new() { Id="skateboard", Name="Скейтборд",           Category="Мото",     Emoji="🛹",  DefaultSpeed=20,  FollowRoads=true,  SvgIcon=SvgIcons.Skateboard },

        // ─── EXOTIC ─────────────────────────────────────────────────
        new() { Id="horse",      Name="Лошадь",              Category="Экзотика", Emoji="🐎",  DefaultSpeed=45,  FollowRoads=false, SvgIcon=SvgIcons.Horse },
        new() { Id="camel",      Name="Верблюд",             Category="Экзотика", Emoji="🐪",  DefaultSpeed=20,  FollowRoads=false, SvgIcon=SvgIcons.Camel },
        new() { Id="elephant",   Name="Слон",                Category="Экзотика", Emoji="🐘",  DefaultSpeed=15,  FollowRoads=false, SvgIcon=SvgIcons.Elephant },
        new() { Id="husky",      Name="Упряжка хаски",       Category="Экзотика", Emoji="🐕",  DefaultSpeed=30,  FollowRoads=false, SvgIcon=SvgIcons.Husky },
        new() { Id="reindeer",   Name="Сани с оленями",      Category="Экзотика", Emoji="🦌",  DefaultSpeed=40,  FollowRoads=false, SvgIcon=SvgIcons.Reindeer },

        // ─── MILITARY ───────────────────────────────────────────────
        new() { Id="apache",     Name="Вертолёт Apache",     Category="Военные",  Emoji="🚁",  DefaultSpeed=290, FollowRoads=false, SvgIcon=SvgIcons.Apache },
        new() { Id="tank",       Name="Танк",                Category="Военные",  Emoji="🪖",  DefaultSpeed=60,  FollowRoads=false, SvgIcon=SvgIcons.Tank },
        new() { Id="warship",    Name="Военный корабль",     Category="Военные",  Emoji="🚢",  DefaultSpeed=55,  FollowRoads=false, SvgIcon=SvgIcons.Warship },

        // ─── FANTASY ────────────────────────────────────────────────
        new() { Id="ufo",        Name="НЛО",                 Category="Фантастика",Emoji="🛸",  DefaultSpeed=9999,FollowRoads=false, SvgIcon=SvgIcons.Ufo },
        new() { Id="magic_carpet",Name="Ковёр-самолёт",      Category="Фантастика",Emoji="🧞",  DefaultSpeed=200, FollowRoads=false, SvgIcon=SvgIcons.MagicCarpet },
        new() { Id="dragon",     Name="Дракон",              Category="Фантастика",Emoji="🐉",  DefaultSpeed=300, FollowRoads=false, SvgIcon=SvgIcons.Dragon },
        new() { Id="broomstick", Name="Метла ведьмы",        Category="Фантастика",Emoji="🧙",  DefaultSpeed=120, FollowRoads=false, SvgIcon=SvgIcons.Broomstick },
    ];
}
