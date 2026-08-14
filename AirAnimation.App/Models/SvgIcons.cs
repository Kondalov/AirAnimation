namespace AirAnimation.App.Models;

/// <summary>
/// Cartoon 3D stylized vector models (TravelBoast signature aesthetic).
/// High-contrast vibrant colors, bold cartoon outlines (cel-shading), 
/// glossy windshields, and spinning propeller discs.
/// All icons point North (0°) and are self-contained without clip-path or ID collisions.
/// </summary>
public static class SvgIcons
{
    // ── AVIATION ─────────────────────────────────────────────────────────────
    // Signature TravelBoast Cartoon Blue Aerobatic Airplane (Screenshots 2, 3, 4)
    public const string Airliner = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <!-- Soft Cartoon Ground Shadow -->
  <ellipse cx="60" cy="74" rx="28" ry="14" fill="rgba(0,0,0,0.35)"/>

  <!-- Wings (Bright Yellow & Blue Layered) -->
  <path d="M 60 44 L 110 58 C 113 59 113 63 110 65 L 102 70 L 60 56 L 18 70 L 10 65 C 7 63 7 59 10 58 Z" 
        fill="#2563EB" stroke="#0F172A" stroke-width="2.5" stroke-linejoin="round"/>
  <!-- Bright Yellow Wingtips -->
  <path d="M 100 55 L 110 58 C 113 59 113 63 110 65 L 102 68 Z" fill="#FACC15" stroke="#0F172A" stroke-width="1.8"/>
  <path d="M 20 55 L 10 58 C 7 59 7 63 10 65 L 18 68 Z" fill="#FACC15" stroke="#0F172A" stroke-width="1.8"/>

  <!-- Tail Stabilizer (Blue with Yellow Tips) -->
  <path d="M 60 98 L 82 106 L 79 112 L 60 106 L 41 112 L 38 106 Z" 
        fill="#1D4ED8" stroke="#0F172A" stroke-width="2"/>
  <polygon points="76,104 82,106 79,112 74,110" fill="#FACC15"/>
  <polygon points="44,104 38,106 41,112 46,110" fill="#FACC15"/>

  <!-- Vertical Tail Fin -->
  <path d="M 58 84 L 62 84 L 61 106 L 59 106 Z" fill="#1E40AF" stroke="#0F172A" stroke-width="1.5"/>

  <!-- Fuselage (Bright Cartoon Blue 3D Body) -->
  <path d="M 60 14 
           C 68 14 73 30 72 58 
           C 71 86 66 106 60 110 
           C 54 106 49 86 48 58 
           C 47 30 52 14 60 14 Z" 
        fill="#3B82F6" stroke="#0F172A" stroke-width="2.5" stroke-linejoin="round"/>

  <!-- Fuselage Top Highlight / Shading (Cartoon 3D Volume) -->
  <path d="M 60 16 C 64 16 67 28 66 52 C 65 74 62 90 60 96 C 58 90 55 74 54 52 C 53 28 56 16 60 16 Z" 
        fill="#60A5FA"/>

  <!-- Cockpit Windshield (Glossy Black/Navy with Bright Cyan Glare) -->
  <path d="M 55 30 C 58 26 62 26 65 30 L 67 44 C 63 42 57 42 53 44 Z" 
        fill="#0F172A" stroke="#1E293B" stroke-width="1.5"/>
  <path d="M 57 31 C 59 28 61 28 63 31 L 64 36 C 62 35 58 35 56 36 Z" 
        fill="#38BDF8"/>
  <ellipse cx="60" cy="40" rx="3" ry="1.2" fill="#FFFFFF" opacity="0.9"/>

  <!-- Yellow Nose Cone Spinner -->
  <ellipse cx="60" cy="15" rx="5" ry="6" fill="#FACC15" stroke="#0F172A" stroke-width="2"/>

  <!-- Spinning Propeller Blur Ring (Cartoon Style) -->
  <ellipse cx="60" cy="13" rx="20" ry="6" fill="rgba(255,255,255,0.6)" stroke="#38BDF8" stroke-width="1.5"/>
  <ellipse cx="50" cy="13" rx="6" ry="1.5" fill="#FFFFFF"/>
  <ellipse cx="70" cy="13" rx="6" ry="1.5" fill="#FFFFFF"/>
  <circle cx="60" cy="13" r="2.5" fill="#0F172A"/>
</svg>
""";

    // Cartoon Red & White Twin-Turboprop Airliner
    public const string BizJet = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="30" ry="14" fill="rgba(0,0,0,0.35)"/>
  <!-- Wings -->
  <path d="M 60 44 L 112 58 L 104 68 L 60 56 L 16 68 L 8 58 Z" 
        fill="#E2E8F0" stroke="#0F172A" stroke-width="2.5" stroke-linejoin="round"/>
  <!-- Tail -->
  <path d="M 60 98 L 84 106 L 80 112 L 60 106 L 40 112 L 36 106 Z" fill="#E2E8F0" stroke="#0F172A" stroke-width="2"/>
  <path d="M 58 84 L 62 84 L 61 106 L 59 106 Z" fill="#E11D48" stroke="#0F172A" stroke-width="1.5"/>
  <!-- Fuselage -->
  <path d="M 60 14 C 68 14 73 30 72 58 C 71 86 66 106 60 110 C 54 106 49 86 48 58 C 47 30 52 14 60 14 Z" 
        fill="#F8FAFC" stroke="#0F172A" stroke-width="2.5"/>
  <!-- Red Livery Band -->
  <path d="M 52 70 C 56 68 64 68 68 70 L 66 94 C 62 92 58 92 54 94 Z" fill="#E11D48"/>
  <!-- Windshield -->
  <path d="M 55 30 C 58 26 62 26 65 30 L 67 44 C 63 42 57 42 53 44 Z" fill="#0F172A"/>
  <ellipse cx="60" cy="38" rx="5" ry="3" fill="#38BDF8"/>
  <!-- Engines with Propellers -->
  <rect x="32" y="46" width="9" height="18" rx="4.5" fill="#E11D48" stroke="#0F172A" stroke-width="1.5"/>
  <ellipse cx="36.5" cy="45" rx="11" ry="3.5" fill="rgba(255,255,255,0.7)" stroke="#38BDF8" stroke-width="1"/>
  <circle cx="36.5" cy="45" r="2" fill="#0F172A"/>
  <rect x="79" y="46" width="9" height="18" rx="4.5" fill="#E11D48" stroke="#0F172A" stroke-width="1.5"/>
  <ellipse cx="83.5" cy="45" rx="11" ry="3.5" fill="rgba(255,255,255,0.7)" stroke="#38BDF8" stroke-width="1"/>
  <circle cx="83.5" cy="45" r="2" fill="#0F172A"/>
</svg>
""";

    // Cartoon Jet Fighter
    public const string Fighter = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="26" ry="14" fill="rgba(0,0,0,0.35)"/>
  <!-- Delta Wings -->
  <path d="M 60 36 L 110 88 L 94 92 L 60 84 L 26 92 L 10 88 Z" fill="#475569" stroke="#0F172A" stroke-width="2.5"/>
  <!-- Fuselage -->
  <path d="M 60 10 L 67 36 L 67 96 L 60 108 L 53 96 L 53 36 Z" fill="#64748B" stroke="#0F172A" stroke-width="2.5"/>
  <!-- Golden Canopy -->
  <ellipse cx="60" cy="42" rx="4" ry="11" fill="#F59E0B" stroke="#0F172A" stroke-width="1.5"/>
  <!-- Flame -->
  <ellipse cx="60" cy="110" rx="3" ry="5" fill="#EF4444"/>
</svg>
""";

    // Cartoon Helicopter
    public const string Helicopter = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="26" ry="14" fill="rgba(0,0,0,0.35)"/>
  <!-- Tail -->
  <path d="M 58 64 L 62 64 L 61 108 L 59 108 Z" fill="#15803D" stroke="#0F172A" stroke-width="2"/>
  <rect x="52" y="104" width="16" height="3" rx="1.5" fill="#F8FAFC"/>
  <!-- Cabin -->
  <ellipse cx="60" cy="54" rx="18" ry="26" fill="#22C55E" stroke="#0F172A" stroke-width="2.5"/>
  <ellipse cx="60" cy="42" rx="13" ry="11" fill="#38BDF8" opacity="0.9"/>
  <!-- Spinning Rotor Disc -->
  <ellipse cx="60" cy="52" rx="48" ry="18" fill="rgba(255,255,255,0.5)" stroke="#38BDF8" stroke-width="1.5"/>
  <circle cx="60" cy="52" r="4" fill="#0F172A"/>
</svg>
""";

    // Cartoon Drone
    public const string Drone = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="26" ry="14" fill="rgba(0,0,0,0.35)"/>
  <line x1="34" y1="34" x2="86" y2="86" stroke="#0F172A" stroke-width="5" stroke-linecap="round"/>
  <line x1="86" y1="34" x2="34" y2="86" stroke="#0F172A" stroke-width="5" stroke-linecap="round"/>
  <circle cx="34" cy="34" r="16" fill="rgba(56,189,248,0.5)" stroke="#38BDF8" stroke-width="1.8"/>
  <circle cx="86" cy="34" r="16" fill="rgba(56,189,248,0.5)" stroke="#38BDF8" stroke-width="1.8"/>
  <circle cx="34" cy="86" r="16" fill="rgba(56,189,248,0.5)" stroke="#38BDF8" stroke-width="1.8"/>
  <circle cx="86" cy="86" r="16" fill="rgba(56,189,248,0.5)" stroke="#38BDF8" stroke-width="1.8"/>
  <rect x="50" y="50" width="20" height="20" rx="6" fill="#F8FAFC" stroke="#0F172A" stroke-width="2"/>
  <circle cx="60" cy="56" r="3.5" fill="#0F172A"/>
</svg>
""";

    // Cartoon Sports Car
    public const string SportsCar = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="24" ry="13" fill="rgba(0,0,0,0.35)"/>
  <path d="M 60 20 C 74 20 82 38 81 74 C 80 98 74 108 60 108 C 46 108 40 98 39 74 C 38 38 46 20 60 20 Z" 
        fill="#E11D48" stroke="#0F172A" stroke-width="2.5"/>
  <path d="M 50 44 C 55 40 65 40 70 44 L 72 66 C 65 62 55 62 48 66 Z" fill="#0F172A"/>
  <ellipse cx="48" cy="28" rx="3.5" ry="5" fill="#FEF08A"/>
  <ellipse cx="72" cy="28" rx="3.5" ry="5" fill="#FEF08A"/>
</svg>
""";

    // Cartoon Passenger Car
    public const string Car = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="24" ry="13" fill="rgba(0,0,0,0.35)"/>
  <path d="M 60 20 C 74 20 82 38 81 74 C 80 98 74 108 60 108 C 46 108 40 98 39 74 C 38 38 46 20 60 20 Z" 
        fill="#2563EB" stroke="#0F172A" stroke-width="2.5"/>
  <path d="M 50 44 C 55 40 65 40 70 44 L 72 66 C 65 62 55 62 48 66 Z" fill="#0F172A"/>
  <ellipse cx="48" cy="28" rx="3.5" ry="5" fill="#FEF08A"/>
  <ellipse cx="72" cy="28" rx="3.5" ry="5" fill="#FEF08A"/>
</svg>
""";

    // Cartoon SUV
    public const string Suv = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="26" ry="14" fill="rgba(0,0,0,0.35)"/>
  <path d="M 60 18 C 76 18 84 38 83 76 C 82 100 76 110 60 110 C 44 110 38 100 37 76 C 36 38 44 18 60 18 Z" 
        fill="#16A34A" stroke="#0F172A" stroke-width="2.5"/>
  <path d="M 48 42 C 54 38 66 38 72 42 L 74 68 C 66 64 54 64 46 68 Z" fill="#0F172A"/>
  <ellipse cx="46" cy="26" rx="3.5" ry="5" fill="#FFF59D"/>
  <ellipse cx="74" cy="26" rx="3.5" ry="5" fill="#FFF59D"/>
</svg>
""";

    // Cartoon Rocket
    public const string Rocket = """
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 120 120" width="100%" height="100%">
  <ellipse cx="60" cy="74" rx="22" ry="13" fill="rgba(0,0,0,0.35)"/>
  <path d="M 60 12 C 68 24 73 52 71 92 L 49 92 C 47 52 52 24 60 12 Z" fill="#F8FAFC" stroke="#0F172A" stroke-width="2.5"/>
  <path d="M 49 70 L 33 98 L 49 92 Z" fill="#E11D48" stroke="#0F172A" stroke-width="2"/>
  <path d="M 71 70 L 87 98 L 71 92 Z" fill="#E11D48" stroke="#0F172A" stroke-width="2"/>
  <circle cx="60" cy="46" r="6" fill="#0284C7" stroke="#0F172A" stroke-width="2"/>
  <path d="M 54 92 Q 60 112 66 92 Z" fill="#F97316"/>
</svg>
""";

    // Fallbacks
    public const string Convertible = Car;
    public const string Pickup = Suv;
    public const string RetroCar = Car;
    public const string Bus = Car;
    public const string Truck = Suv;
    public const string Van = Suv;
    public const string Tractor = Suv;
    public const string Glider = Airliner;
    public const string Airship = Airliner;
    public const string HotBalloon = Airliner;
    public const string Seaplane = Airliner;
    public const string Shuttle = Rocket;
    public const string Satellite = Drone;
    public const string Yacht = BizJet;
    public const string CruiseShip = Airliner;
    public const string Ferry = Airliner;
    public const string Submarine = Airliner;
    public const string Motorboat = SportsCar;
    public const string Kayak = SportsCar;
    public const string Train = Airliner;
    public const string TGV = Airliner;
    public const string Metro = Airliner;
    public const string Tram = Airliner;
    public const string Monorail = Airliner;
    public const string Motorcycle = SportsCar;
    public const string Scooter = SportsCar;
    public const string Bicycle = SportsCar;
    public const string EScooter = SportsCar;
    public const string Skateboard = SportsCar;
    public const string Horse = SportsCar;
    public const string Camel = SportsCar;
    public const string Elephant = SportsCar;
    public const string Husky = SportsCar;
    public const string Reindeer = SportsCar;
    public const string Apache = Helicopter;
    public const string Tank = Suv;
    public const string Warship = Airliner;
    public const string Ufo = Drone;
    public const string MagicCarpet = SportsCar;
    public const string Dragon = Fighter;
    public const string Broomstick = Fighter;
}
