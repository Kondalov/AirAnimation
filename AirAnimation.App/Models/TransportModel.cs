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
        // ─── AUTO ───────────────────────────────────────────────────
        new() { Id="car",        Name="Легковой автомобиль", Category="Авто",     Emoji="🚗",  DefaultSpeed=90,  FollowRoads=true,  SvgIcon=SvgIcons.Car },
        new() { Id="suv",        Name="Внедорожник",         Category="Авто",     Emoji="🚙",  DefaultSpeed=85,  FollowRoads=true,  SvgIcon=SvgIcons.Suv },
        new() { Id="sports_car", Name="Спорткар",            Category="Авто",     Emoji="🏎️", DefaultSpeed=150, FollowRoads=true,  SvgIcon=SvgIcons.SportsCar },
        new() { Id="convertible",Name="Кабриолет",           Category="Авто",     Emoji="🚗",  DefaultSpeed=110, FollowRoads=true,  SvgIcon=SvgIcons.Convertible },
        new() { Id="pickup",     Name="Пикап",               Category="Авто",     Emoji="🛻",  DefaultSpeed=80,  FollowRoads=true,  SvgIcon=SvgIcons.Pickup },
        new() { Id="retro_car",  Name="Ретро-автомобиль",    Category="Авто",     Emoji="🚘",  DefaultSpeed=60,  FollowRoads=true,  SvgIcon=SvgIcons.RetroCar },
        new() { Id="bus",        Name="Автобус",             Category="Авто",     Emoji="🚌",  DefaultSpeed=70,  FollowRoads=true,  SvgIcon=SvgIcons.Bus },
        new() { Id="truck",      Name="Грузовик",            Category="Авто",     Emoji="🚛",  DefaultSpeed=75,  FollowRoads=true,  SvgIcon=SvgIcons.Truck },
        new() { Id="van",        Name="Минивэн",             Category="Авто",     Emoji="🚐",  DefaultSpeed=80,  FollowRoads=true,  SvgIcon=SvgIcons.Van },
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
