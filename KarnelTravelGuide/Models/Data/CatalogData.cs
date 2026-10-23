using KarnelTravelGuide.Models;

namespace KarnelTravelGuide.Data;

// Raw site content only (no logic): packages, places, category titles, home slides and tiles.
// Queries on this data live in Models/Services/CatalogService.cs. Edit the lists below to change the site content.
public static class CatalogData
{
    //                                  name, offer, photo keyword, SPOTS, STAYS, RESTAURANTS, TRAVEL
    // Names inside a package must match an item below (checked at start-up).
    public static readonly TourPackage[] Packages =
    {
        new("Northern Areas - 7 Days", "20% OFF", "mountains,lake",
            new[]{ "Hunza Valley", "Fairy Meadows", "Deosai National Park", "Neelum Valley" },
            new[]{ "Shangrila Resort", "Hunza Serene Resort", "Naran Mountain Resort" },
            new[]{ "Mountain Cafe", "Monal", "Dhaba Tea House" },
            new[]{ "Mountain Jeep Transfer", "Tourist Shuttle" }),
        new("Karachi Beach Weekend", "15% OFF", "beach,sunset",
            new[]{ "Clifton Beach", "Kund Malir Beach" },
            new[]{ "Pearl Continental" },
            new[]{ "Kolachi", "Sea Breeze Grill" },
            new[]{ "Airport Transfer" }),
        new("Lahore Heritage Tour", "10% OFF", "mosque,fort",
            new[]{ "Badshahi Mosque", "Lahore Fort" },
            new[]{ "Old City Heritage Hotel" },
            new[]{ "Haveli Restaurant", "Butt Karahi", "Lal Qila Restaurant" },
            new[]{ "Private Car & Driver" }),
        new("Summer Northern Tour", "20% OFF 7-Day Trips", "mountains,camping",
            new[]{ "Shogran", "Fairy Meadows", "Neelum Valley" },
            new[]{ "Kaghan Valley Resort", "Murree Pine Resort" },
            new[]{ "Dhaba Tea House", "Mountain Cafe" },
            new[]{ "Mountain Jeep Transfer", "Group Coaster Van" }),
        new("Family Beach Package", "Kids Stay Free", "beach,family",
            new[]{ "Clifton Beach", "Kund Malir Beach" },
            new[]{ "Seaview Residency", "Gwadar Sea Resort" },
            new[]{ "Sea Breeze Grill", "Xander's" },
            new[]{ "Group Coaster Van" }),
        new("Early Bird Booking", "10% OFF When You Book 30 Days Early", "airplane,sky",
            new[]{ "Faisal Mosque", "Rohtas Fort" },
            new[]{ "Margalla View Hotel", "Mall Road Inn" },
            new[]{ "Monal" },
            new[]{ "Airport Transfer" }),
    };

    public static readonly Dictionary<string, string> Titles = new()
    {
        ["tourist-spots"] = "Tourist Spots", ["hotels"] = "Hotels", ["restaurants"] = "Restaurants",
        ["resorts"] = "Resorts", ["travel"] = "Travel Information"
    };

    static Item I(string c, string n, string city, int p, double r, int q, bool a, string d, string k) => new(c, n, city, p, r, q, a, d, k);

    public static readonly List<Item> All = new()
    {
        I("tourist-spots","Hunza Valley","Gilgit-Baltistan",5000,4.9,40,true,"Snow-capped peaks, apricot orchards and the ancient Baltit Fort.","mountains"),
        I("tourist-spots","Clifton Beach","Karachi",500,4.1,500,true,"Sunset walks, camel rides and a fresh sea breeze in the heart of the city.","beach"),
        I("tourist-spots","Badshahi Mosque","Lahore",300,4.8,500,true,"A Mughal-era masterpiece with a vast red-sandstone courtyard.","mosque"),
        I("tourist-spots","Fairy Meadows","Diamer",7000,4.9,30,true,"Green meadows with a front-row view of Nanga Parbat.","meadow"),
        I("tourist-spots","Mohenjo-daro","Larkana",400,4.5,200,true,"A UNESCO World Heritage Site: the ruins of a 4,500-year-old Indus Valley city.","ruins"),
        I("tourist-spots","Lahore Fort","Lahore",400,4.7,300,true,"A royal fort featuring the Sheesh Mahal and grand gardens.","fort"),
        I("tourist-spots","Kund Malir Beach","Balochistan",1500,4.6,100,true,"Golden sand and clear blue water along the Makran Coastal Highway.","seashore"),
        I("tourist-spots","Neelum Valley","Azad Kashmir",6000,4.8,50,true,"Rivers, forests and mountain villages surrounded by lush greenery.","valley"),
        I("hotels","Pearl Continental","Karachi",28000,4.6,60,true,"A five-star stay with a pool, spa and fine dining.","hotel"),
        I("hotels","Serena Hotel","Islamabad",35000,4.8,35,true,"Elegant rooms near the Margalla Hills.","luxuryhotel"),
        I("hotels","Avari Towers","Lahore",24000,4.5,0,false,"Central Lahore comfort with skyline views.","hotelroom"),
        I("hotels","Marriott Hotel","Islamabad",32000,4.7,45,true,"Business-class rooms with rooftop views.","hotellobby"),
        I("hotels","Movenpick Hotel","Karachi",26000,4.5,50,true,"Modern rooms close to the city's main attractions.","hotelbed"),
        I("restaurants","Kolachi","Karachi",3500,4.6,80,true,"Seaside dining with BBQ, karahi and a sea view.","restaurant"),
        I("restaurants","Monal","Islamabad",4500,4.7,120,true,"A hilltop restaurant overlooking the entire capital.","dinner"),
        I("restaurants","Bundu Khan","Lahore",1800,4.4,150,true,"Classic Lahori BBQ, seekh kebab and tikka.","barbecue"),
        I("restaurants","Cafe Aylanto","Lahore",5000,4.5,0,false,"A stylish café serving continental and fusion dishes.","cafe"),
        I("restaurants","Lal Qila Restaurant","Lahore",3000,4.3,90,true,"Desi feasts with a royal Mughal feel.","curry"),
        I("resorts","Shangrila Resort","Skardu",22000,4.8,25,true,"Lakeside cottages known as 'Heaven on Earth'.","lake"),
        I("resorts","Serena Shigar Fort","Skardu",30000,4.9,15,true,"A luxury mountain retreat with views of the snow-capped Karakoram.","castle"),
        I("resorts","Pearl Continental Bhurban","Murree",25000,4.6,40,true,"A pine-covered mountain resort with a golf course and spa.","resort"),
        I("resorts","Swat Riverside Resort","Swat",15000,4.4,30,true,"Rooms beside the Swat River, surrounded by green hills.","river"),
        I("resorts","Naran Mountain Resort","Naran",12000,4.3,0,false,"Cosy chalets near Saif-ul-Muluk Lake.","chalet"),
        I("travel","Daewoo Express Bus","Nationwide",3500,4.4,200,true,"Comfortable inter-city coaches on all major routes.","bus"),
        I("travel","Green Line Train","Karachi - Islamabad",6500,4.5,120,true,"An air-conditioned train with sleeper and business class.","train"),
        I("travel","Domestic Flights","Major cities",18000,4.6,150,true,"Fast flights between Karachi, Lahore, Islamabad and Skardu.","airplane"),
        I("travel","Private Car & Driver","Nationwide",15000,4.7,30,true,"A private car with an experienced driver, hired by the day.","car"),
        I("travel","Mountain Jeep Transfer","Northern Areas",9000,4.6,20,true,"4x4 jeeps for rough mountain routes, such as the road to Fairy Meadows.","jeep"),
        I("tourist-spots","Faisal Mosque","Islamabad",200,4.8,500,true,"An iconic tent-shaped mosque at the foot of the Margalla Hills.","minaret"),
        I("tourist-spots","Deosai National Park","Skardu",3000,4.9,60,true,"Known as the 'Land of Giants', with wildflowers at 4,000 metres.","wildflowers"),
        I("tourist-spots","Rohtas Fort","Jhelum",300,4.4,200,true,"A massive 16th-century fort and UNESCO World Heritage Site.","ancient"),
        I("tourist-spots","Shogran","Kaghan",4000,4.5,70,true,"Pine forests and the trek to the Siri Paye meadows.","forest"),
        I("hotels","Regent Plaza","Karachi",21000,4.4,40,true,"Comfortable rooms near Shahrah-e-Faisal.","hotelview"),
        I("hotels","Pearl Continental Lahore","Lahore",27000,4.6,55,true,"A grand stay with a pool and several restaurants.","hotelpool"),
        I("hotels","Ramada Hotel","Islamabad",19000,4.3,30,true,"A modern business hotel close to the airport road.","suite"),
        I("restaurants","Xander's","Karachi",3800,4.3,70,true,"Steaks, pasta and a relaxed café atmosphere.","steak"),
        I("restaurants","Haveli Restaurant","Lahore",4200,4.6,100,true,"Rooftop dining with a view of Badshahi Mosque.","rooftop"),
        I("restaurants","Student Biryani","Karachi",900,4.2,200,true,"Karachi's famous spicy biryani, served fresh and fast.","biryani"),
        I("restaurants","Dhaba Tea House","Islamabad",1200,4.1,80,true,"Chai, parathas and desi snacks in a cosy setting.","tea"),
        I("resorts","Hunza Serene Resort","Hunza",18000,4.7,25,true,"Rooms with views of the Rakaposhi and Ultar peaks.","mountainresort"),
        I("resorts","Kaghan Valley Resort","Kaghan",14000,4.4,30,true,"Wooden cottages beside the river and pine forest.","cottage"),
        I("resorts","Kund Malir Beach Huts","Balochistan",9000,4.2,20,true,"Simple beach huts right on the golden sand.","beachhut"),
        I("resorts","Neelum Riverside Huts","Azad Kashmir",11000,4.5,18,true,"Wooden huts on the bank of the Neelum River.","cabin"),
        I("travel","City Ride Hailing","Major cities",1500,4.5,500,true,"Taxi and ride-hailing services within the city.","taxi"),
        I("travel","Group Coaster Van","Nationwide",12000,4.6,40,true,"Coaster vans for families and groups, hired by the day.","van"),
        I("travel","Bike Adventure Rental","Nationwide",4000,4.3,25,true,"Touring motorbikes for adventurous riders.","motorbike"),
        I("travel","Helicopter Sightseeing","Northern Areas",45000,4.8,10,true,"Aerial views of the mighty Karakoram peaks.","helicopter"),
        I("hotels","Margalla View Hotel","Islamabad",17000,4.3,30,true,"Balcony rooms facing the Margalla Hills.","hotelbalcony"),
        I("hotels","Seaview Residency","Karachi",16000,4.2,35,true,"A comfortable stay a short walk from the beach.","seaviewhotel"),
        I("hotels","Mall Road Inn","Murree",14000,4.2,25,true,"A cosy mountain hotel on the famous Mall Road.","mountainhotel"),
        I("hotels","Old City Heritage Hotel","Lahore",18000,4.5,20,true,"Restored haveli rooms inside the Walled City.","heritagehotel"),
        I("restaurants","Butt Karahi","Lahore",1800,4.4,120,true,"Sizzling chicken and mutton karahi, Lahori style.","karahi"),
        I("restaurants","Sea Breeze Grill","Karachi",3200,4.2,80,true,"Grilled seafood and BBQ with a cool ocean breeze.","grill"),
        I("restaurants","Mountain Cafe","Hunza",1500,4.5,40,true,"Warm coffee and local dishes with valley views.","coffee"),
        I("resorts","Murree Pine Resort","Murree",16000,4.3,35,true,"Rooms among the pine trees, with bonfire evenings.","pinetrees"),
        I("resorts","Skardu Lakeview Huts","Skardu",13000,4.4,22,true,"Wooden huts right beside Upper Kachura Lake.","lakehut"),
        I("resorts","Gwadar Sea Resort","Gwadar",20000,4.3,28,true,"Sea-facing suites on the Arabian Sea coast.","tropicalbeach"),
        I("travel","Airport Transfer","Major cities",2500,4.6,300,true,"Punctual airport pickup and drop-off with trained drivers.","airport"),
        I("travel","Luxury Sedan Hire","Nationwide",20000,4.7,20,true,"Premium sedans with a chauffeur for special occasions.","limousine"),
        I("travel","Tourist Shuttle","Islamabad - Hunza",8000,4.4,50,true,"A scheduled minibus shuttle to the northern areas.","minibus"),
    };

    public static readonly Slide[] Slides =
    {
        new("Hunza",  "Hunza Valley",   "hunza", 5, "Turquoise lakes, apricot orchards and towering snow peaks. Hunza Valley is the postcard of northern Pakistan, ready for your next getaway."),
        new("Skardu", "Shangrila, Skardu",     "skardu",     5, "The gateway to K2, with cold deserts, glassy lakes and historic forts. Plan a trip full of adventure and peaceful mountain evenings."),
        new("Swat",   "Swat Valley",     "swat",     4, "The Switzerland of the East, with green meadows, rivers and cosy hill hotels. Perfect for families and weekend escapes."),
        new("Gwadar", "Gwadar Port",    "gwadar",     4, "Golden cliffs meet the Arabian Sea. Enjoy quiet beaches, fresh seafood and sunsets you will never forget."),
        new("Naran",  "Kaghan Valley, Naran", "naran",    5, "Alpine lakes, pine forests and fresh mountain air. Book your stay and explore the legendary Kaghan Valley.")
    };

    public static readonly (string Href, string Title, string Kw)[] Tiles =
    {
        ("/Packages","Tourist Spots","landmark"), ("/Packages","Hotels & Resorts","hotel"), ("/Packages","Restaurants","restaurant"),
        ("/Packages","Travel Guide","transport"), ("/Packages","Packages & Offers","holiday")
    };
}
