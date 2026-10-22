-- =============================================================================
-- Seed script for Vector Recommendation Engine (recsys."CatalogItems")
-- Categories: 0: Electronic, 1: Toys, 2: Clothes, 3: Garden, 4: Other
-- Vector: vector(3) normalized unit vectors ([x, y, z] where x^2 + y^2 + z^2 = 1.0)
-- =============================================================================

INSERT INTO recsys."CatalogItems" ("Id", "Title", "Category", "Description", "Embedding", "PopularityScore", "CreatedAt")
VALUES
    -- Electronic (Category 0)
    ('eci_0192a001-0001-7000-8000-000000000001', 'MacBook Pro M3 Max 16"', 0, 'Wydajny laptop dla programistów i profesjonalistów z ekranem Liquid Retina XDR.', '[0.96, 0.19, 0.20]', 450, NOW()),
    ('eci_0192a001-0002-7000-8000-000000000002', 'Dell XPS 15 OLED', 0, 'Ultrabook z ekranem 4K OLED, procesorem Intel Core i9 i dedykowaną grafiką RTX.', '[0.94, 0.24, 0.24]', 320, NOW()),
    ('eci_0192a001-0003-7000-8000-000000000003', 'Sony WH-1000XM5 ANC', 0, 'Bezprzewodowe słuchawki nagłowne z wiodącą w branży redukcją hałasu.', '[0.88, 0.35, 0.32]', 280, NOW()),
    ('eci_0192a001-0004-7000-8000-000000000004', 'Keychron Q1 Pro Wireless Mechanical', 0, 'W pełni aluminiowa klawiatura mechaniczna z przełącznikami hot-swap.', '[0.85, 0.40, 0.34]', 190, NOW()),
    ('eci_0192a001-0005-7000-8000-000000000005', 'iPad Pro 13" M4 OLED', 0, 'Ultra-cienki tablet z ekranem Tandem OLED i obsługą Apple Pencil Pro.', '[0.92, 0.28, 0.27]', 390, NOW()),

    -- Toys (Category 1)
    ('eci_0192a001-0006-7000-8000-000000000006', 'LEGO Icons NASA Space Shuttle Discovery', 1, 'Kolekcjonerski zestaw klocków z promem kosmicznym i teleskopem Hubble.', '[0.22, 0.95, 0.22]', 520, NOW()),
    ('eci_0192a001-0007-7000-8000-000000000007', 'LEGO Technic Ferrari Daytona SP3', 1, 'Zaawansowany model w skali 1:8 z działającą skrzynią biegów i silnikiem V12.', '[0.25, 0.94, 0.24]', 410, NOW()),
    ('eci_0192a001-0008-7000-8000-000000000008', 'DJI Mini 4 Pro Drone', 1, 'Kompaktowy dron z wielokierunkowym wykrywaniem przeszkód i wideo 4K/60fps.', '[0.38, 0.88, 0.27]', 340, NOW()),
    ('eci_0192a001-0009-7000-8000-000000000009', 'Gra planszowa Terraforming Mars', 1, 'Strategiczna gra planszowa o terraformacji Czerwonej Planety.', '[0.18, 0.92, 0.34]', 290, NOW()),
    ('eci_0192a001-0010-7000-8000-000000000010', 'Nintendo Switch OLED Model', 1, 'Hybrydowa konsola do gier z 7-calowym ekranem OLED.', '[0.32, 0.91, 0.26]', 480, NOW()),

    -- Clothes (Category 2)
    ('eci_0192a001-0011-7000-8000-000000000011', 'Kurtka Puchowa Patagonia Down Sweater', 2, 'Lekka i wiatroszczelna kurtka outdoorowa z puchem gęsim 800-fill.', '[0.20, 0.22, 0.95]', 310, NOW()),
    ('eci_0192a001-0012-7000-8000-000000000012', 'Buty biegowe Hoka Clifton 9', 2, 'Maksymalnie amortyzowane buty treningowe na długie dystanse asfaltowe.', '[0.24, 0.28, 0.92]', 260, NOW()),
    ('eci_0192a001-0013-7000-8000-000000000013', 'Bluza z kapturem Arc''teryx Cormac Hoody', 2, 'Funkcyjna bluza techniczna chroniąca przed słońcem i wiatrem.', '[0.18, 0.20, 0.96]', 210, NOW()),
    ('eci_0192a001-0014-7000-8000-000000000014', 'Spodnie trekkingowe Fjallraven Vidda Pro', 2, 'Trwałe spodnie z materiału G-1000 z profilowanymi kolanami.', '[0.22, 0.30, 0.93]', 275, NOW()),
    ('eci_0192a001-0015-7000-8000-000000000015', 'Czapka zimowa Carhartt Acrylic Watch', 2, 'Klasyczna, ciepła czapka dokerka z prążkowanej dzianiny.', '[0.15, 0.24, 0.96]', 380, NOW()),

    -- Garden (Category 3)
    ('eci_0192a001-0016-7000-8000-000000000016', 'Robot koszący Gardena Smart Sileno', 3, 'Autonomiczny robot do trawników o powierzchni do 1000 m2 sterowany aplikacją.', '[0.62, 0.35, 0.70]', 350, NOW()),
    ('eci_0192a001-0017-7000-8000-000000000017', 'Karcher K7 Premium Smart Control', 3, 'Wysokociśnieniowa myjka ogrodowa z pistoletem Bluetooth i systemem zwijania węża.', '[0.58, 0.42, 0.69]', 290, NOW()),
    ('eci_0192a001-0018-7000-8000-000000000018', 'Sekator akumulatorowy Bosch Pro Pruner', 3, 'Precyzyjny sekator do gałęzi z silnikiem bezszczotkowym.', '[0.52, 0.38, 0.76]', 170, NOW()),
    ('eci_0192a001-0019-7000-8000-000000000019', 'Zestaw mebli ogrodowych z technorattanu', 3, 'Wygodny zestaw narożny z poduszkami odpornymi na warunki atmosferyczne.', '[0.35, 0.45, 0.82]', 240, NOW()),
    ('eci_0192a001-0020-7000-8000-000000000020', 'Szklarnia poliwęglanowa 3x6m', 3, 'Wzmocniona konstrukcja ze stali ocynkowanej i poliwęglanu komorowego.', '[0.40, 0.35, 0.84]', 160, NOW()),

    -- Other (Category 4)
    ('eci_0192a001-0021-7000-8000-000000000021', 'Ekspres ciśnieniowy DeLonghi Magnifica S', 4, 'Automatyczny ekspres do kawy ze spieniaczem mleka i młynkiem stalowym.', '[0.60, 0.55, 0.58]', 490, NOW()),
    ('eci_0192a001-0022-7000-8000-000000000022', 'Oczyszczacz powietrza Dyson Purifier Hot+Cool', 4, 'Urządzenie 3w1: oczyszczacz z filtrem HEPA H13, wentylator i termowentylator.', '[0.68, 0.45, 0.58]', 310, NOW()),
    ('eci_0192a001-0023-7000-8000-000000000023', 'Kubek termiczny Ember Mug 2', 4, 'Inteligentny kubek podgrzewany bateryjnie utrzymujący temperaturę napoju.', '[0.65, 0.50, 0.57]', 220, NOW()),
    ('eci_0192a001-0024-7000-8000-000000000024', 'Zestaw noży kuchennych Wusthof Classic 6szt', 4, 'Kute noże ze stali nierdzewnej w bloku z drewna bukowego.', '[0.50, 0.60, 0.62]', 180, NOW()),
    ('eci_0192a001-0025-7000-8000-000000000025', 'Smartwatch Garmin Fenix 7 Pro Sapphire Solar', 4, 'Wytrzymały zegarek multisportowy z ładowaniem solarnym i latarką LED.', '[0.72, 0.48, 0.50]', 440, NOW())
ON CONFLICT ("Id") DO NOTHING;
