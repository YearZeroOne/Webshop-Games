function populateCitiesByCountry() {

// Map each country to its corresponding cities
const citiesByCountry = {

    "Afghanistan": ['Kabul', 'Kandahar', 'Herat', 'Mazar-i-Sharif', 'Jalalabad'],
    "Albania": ['Tirana', 'Durrës', 'Vlorë', 'Elbasan', 'Shkodër'],
    "Algeria": ['Algiers', 'Oran', 'Constantine', 'Annaba', 'Blida'],
    "Andorra": ['Andorra la Vella', 'Escaldes-Engordany', 'Encamp', 'Sant Julià de Lòria', 'La Massana'],
    "Angola": ['Luanda', 'Huambo', 'Lobito', 'Benguela', 'Kuito'],
    "Antigua and Barbuda": ['St. John\'s', 'All Saints', 'Liberta', 'Bolans', 'Potters Village'],
    "Saudi Arabia": ['Riyadh', 'Jeddah', 'Mecca', 'Medina', 'Dammam'],
    "Argentina": ['Buenos Aires', 'Córdoba', 'Rosario', 'Mendoza', 'La Plata'],
    "Armenia": ['Yerevan', 'Gyumri', 'Vanadzor', 'Ejmiatsin', 'Hrazdan'],
    "Australia": ['Sydney', 'Melbourne', 'Brisbane', 'Perth', 'Adelaide'],
    "Austria": ['Vienna', 'Graz', 'Linz', 'Salzburg', 'Innsbruck', 'Klagenfurt', 'Wels', 'Sankt Pölten', 'Dornbirn', 'Wiener Neustadt'],
    "Azerbaijan": ['Baku', 'Ganja', 'Sumqayit', 'Mingachevir', 'Lankaran'],
    "Bahamas": ['Nassau', 'Freeport', 'West End', 'Coopers Town', 'Marsh Harbour'],
    "Bahrain": ['Manama', 'Muharraq', 'Riffa', 'Isa Town', 'Hamad Town'],
    "Bangladesh": ['Dhaka', 'Chittagong', 'Khulna', 'Rajshahi', 'Comilla'],
    "Barbados": ['Bridgetown', 'Speightstown', 'Oistins', 'Bathsheba', 'Holetown'],
    "Belgium": ['Brussels', 'Antwerp', 'Ghent', 'Charleroi', 'Liège'],
    "Belize": ['Belmopan', 'Belize City', 'San Ignacio', 'Orange Walk', 'Corozal Town'],
    "Benin": ['Cotonou', 'Porto-Novo', 'Parakou', 'Djougou', 'Bohicon'],
    "Bhutan": ['Thimphu', 'Phuntsholing', 'Paro', 'Punakha', 'Samdrup Jongkhar'],
    "Belarus": ['Minsk', 'Gomel', 'Mogilev', 'Vitebsk', 'Grodno'],
    "Myanmar (Burma)": ['Yangon', 'Mandalay', 'Naypyidaw', 'Mawlamyine', 'Bago'],
    "Bolivia": ['La Paz', 'Cochabamba', 'Santa Cruz de la Sierra', 'El Alto', 'Oruro'],
    "Bosnia and Herzegovina": ['Sarajevo', 'Banja Luka', 'Tuzla', 'Zenica', 'Mostar'],
    "Botswana": ['Gaborone', 'Francistown', 'Molepolole', 'Serowe', 'Kanye', 'Mahalapye', 'Mogoditshane', 'Lobatse', 'Thamaga', 'Mochudi', 'Letlhakane', 'Tonota', 'Ramotswa', 'Palapye', 'Maun', 'Mosopa', 'Kasane', 'Tshabong', 'Lehututu', 'Bobonong'],
    "Brazil": ['São Paulo', 'Rio de Janeiro', 'Brasília', 'Salvador', 'Fortaleza'],
    "Brunei": ['Bandar Seri Begawan', 'Kuala Belait', 'Seria', 'Tutong', 'Bangar'],
    "Bulgaria": ['Sofia', 'Plovdiv', 'Varna', 'Burgas', 'Ruse'],
    "Burkina Faso": ['Ouagadougou', 'Bobo-Dioulasso', 'Koudougou', 'Ouahigouya', 'Banfora'],
    "Burundi": ['Bujumbura', 'Gitega', 'Ngozi', 'Ruyigi', 'Muyinga'],
    "Canada": ['Toronto', 'Montreal', 'Vancouver', 'Calgary', 'Ottawa'],
    "Chile": ['Santiago', 'Valparaíso', 'Concepción', 'Antofagasta', 'Viña del Mar'],
    "China": ['Shanghai', 'Beijing', 'Guangzhou', 'Shenzhen', 'Wuhan', 'Tianjin', 'Chongqing', 'Chengdu', 'Nanjing', 'Xi’an', 'Hangzhou', 'Harbin', 'Shenyang', 'Suzhou', 'Qingdao', 'Dalian', 'Zhengzhou', 'Jinan', 'Changsha', 'Fuzhou', 'Ningbo', 'Wuxi', 'Hefei', 'Changchun', 'Jiaxing', 'Xiamen', 'Kunming', 'Nanchang', 'Taiyuan', 'Changzhou', 'Lanzhou', 'Nanning', 'Ürümqi', 'Guiyang', 'Haikou', 'Shijiazhuang', 'Yantai', 'Hohhot', 'Zibo', 'Huai’an', 'Wuhu', 'Jining', 'Yangzhou', 'Weifang', 'Zaozhuang', 'Jiangmen', 'Zhangjiakou', 'Linyi', 'Nanping', 'Huzhou', 'Yiwu', 'Zhangzhou', 'Xuzhou', 'Dandong', 'Jinzhou', 'Anshan', 'Fushun', 'Qinhuangdao', 'Yancheng', 'Shaoxing', 'Quzhou', 'Taizhou', 'Lishui', 'Suqian', 'Loudi', 'Zhuzhou', 'Yueyang', 'Xiangtan', 'Yichang', 'Huangshi', 'Wuhai', 'Jiayuguan', 'Yinchuan', 'Tongliao', 'Ordos', 'Bayannur', 'Wulanchabu', 'Chifeng', 'Erenhot', 'Jilin', 'Huainan', 'Benxi', 'Huludao', 'Tonghua', 'Daqing', 'Qiqihar', 'Suining', 'Zigong', 'Deyang', 'Leshan', 'Neijiang', 'Yibin', 'Wanzhou', 'Dazhou', 'Yichun', 'Shangrao', 'Ganzhou', 'Yingtan', 'Fuzhou', 'Xinyu', 'Jiujiang', 'Lianyungang', 'Yixing', 'Yangzhou', 'Nantong', 'Haimen', 'Qidong', 'Taicang', 'Zhangjiagang', 'Jiaxing', 'Cixi', 'Fuyang', 'Tongling', 'Suzhou', 'Tongren', 'Zunyi', 'Luzhou', 'Chongzuo', 'Hezhou', 'Laibin', 'Liuzhou', 'Changzhi', 'Datong', 'Jincheng', 'Ji’an', 'Ganzhou', 'Baotou', 'Xilinhot', 'Alxa Left Banner', 'Hulunbuir', 'Zhalantun', 'Xingyi', 'Anshun', 'Qujing', 'Yuxi', 'Kaili', 'Dali', 'Baoshan', 'Lijiang', 'Kunshan', 'Zhangqiao', 'Laiwu', 'Linqing', 'Nanxun'],
    "Croatia": ['Zagreb', 'Split', 'Rijeka', 'Osijek', 'Pula'],
    "Cyprus": ['Nicosia', 'Limassol', 'Larnaca', 'Famagusta', 'Paphos'],
    "Czech Republic": ['Prague', 'Brno', 'Ostrava', 'Pilsen', 'Liberec'],
    "Denmark": ['Copenhagen', 'Aarhus', 'Odense', 'Aalborg', 'Esbjerg'],
    "Djibouti": ['Djibouti', 'Ali Sabieh', 'Tadjoura', 'Obock', 'Dikhil'],
    "Dominica": ['Roseau', 'Portsmouth', 'Marigot', 'Berekua', 'Mahaut'],
    "Dominican Republic": ['Santo Domingo', 'Santiago', 'San Pedro de Macorís', 'La Romana', 'San Cristóbal'],
    "Ecuador": ['Quito', 'Guayaquil', 'Cuenca', 'Ambato', 'Manta'],
    "Egypt": ['Cairo', 'Alexandria', 'Giza', 'Shubra El-Kheima', 'Port Said'],
    "El Salvador": ['San Salvador', 'Santa Ana', 'Soyapango', 'San Miguel', 'Mejicanos'],
    "Equatorial Guinea": ['Malabo', 'Bata', 'Ebebiyín', 'Aconibe', 'Anisoc'],
    "Eritrea": ['Asmara', 'Keren', 'Massawa', 'Assab', 'Mendefera'],
    "Estonia": ['Tallinn', 'Tartu', 'Narva', 'Pärnu', 'Viljandi'],
    "Ethiopia": ['Addis Ababa', 'Dire Dawa', 'Mekelle', 'Nazret', 'Bahir Dar'],
    "Fiji": ['Suva', 'Lautoka', 'Nadi', 'Labasa', 'Levuka'],
    "Finland": ['Helsinki', 'Espoo', 'Tampere', 'Vantaa', 'Oulu'],
    "France": ['Paris', 'Marseille', 'Lyon', 'Toulouse', 'Nice'],
    "Germany": ['Berlin', 'Munich', 'Frankfurt', 'Hamburg', 'Cologne', 'Stuttgart', 'Düsseldorf', 'Dortmund', 'Essen', 'Leipzig', 'Bremen', 'Dresden', 'Hanover', 'Nuremberg', 'Duisburg', 'Bochum', 'Wuppertal', 'Bielefeld', 'Bonn', 'Münster', 'Karlsruhe', 'Mannheim', 'Augsburg', 'Wiesbaden', 'Gelsenkirchen', 'Mönchengladbach', 'Braunschweig', 'Chemnitz', 'Kiel', 'Aachen', 'Halle', 'Magdeburg', 'Freiburg', 'Krefeld', 'Lübeck', 'Oberhausen', 'Erfurt', 'Mainz', 'Rostock', 'Kassel', 'Hagen', 'Saarbrücken', 'Hamm', 'Mülheim an der Ruhr', 'Potsdam', 'Ludwigshafen', 'Oldenburg', 'Leverkusen', 'Neuss', 'Paderborn', 'Heidelberg', 'Darmstadt', 'Würzburg', 'Regensburg', 'Ingolstadt', 'Heilbronn', 'Ulm', 'Göttingen', 'Bottrop', 'Trier', 'Recklinghausen', 'Offenbach', 'Fürth', 'Moers', 'Siegen', 'Cottbus', 'Koblenz', 'Bergisch Gladbach', 'Gera', 'Bremerhaven', 'Reutlingen', 'Remscheid', 'Salzgitter', 'Jena', 'Gütersloh', 'Kaiserslautern', 'Witten', 'Hanau', 'Schwerin', 'Düren', 'Ratingen', 'Esslingen', 'Lünen', 'Castrop-Rauxel', 'Lüneburg', 'Brandenburg an der Havel', 'Lüdenscheid', 'Marl', 'Villingen-Schwenningen', 'Hattingen', 'Herford', 'Bünde', 'Grevenbroich', 'Friedrichshafen', 'Rheine', 'Stolberg', 'Gladbeck', 'Rheda-Wiedenbrück', 'Bad Salzuflen', 'Pulheim', 'Herten', 'Singen', 'Suhl', 'Rendsburg', 'Schleswig', 'Wismar', 'Straubing', 'Gifhorn', 'Neumünster', 'Schwäbisch Gmünd', 'Emsdetten', 'Höxter', 'Winsen', 'Zittau', 'Peine', 'Limbach-Oberfrohna', 'Bühl', 'Neunkirchen'],
    "Georgia": ['Tbilisi', 'Kutaisi', 'Batumi', 'Rustavi', 'Zugdidi'],
    "Ghana": ['Accra', 'Kumasi', 'Tamale', 'Takoradi', 'Cape Coast'],
    "Greece": ['Athens', 'Thessaloniki', 'Patras', 'Heraklion', 'Larissa'],
    "Grenada": ['St. George\'s', 'Gouyave', 'Victoria', 'Sauteurs', 'Hillsborough'],
    "Guatemala": ['Guatemala City', 'Mixco', 'Villa Nueva', 'Quetzaltenango', 'San Juan Sacatepéquez'],
    "Guinea": ['Conakry', 'Nzérékoré', 'Kindia', 'Kankan', 'Gueckedou'],
    "Guinea-Bissau": ['Bissau', 'Bafatá', 'Gabú', 'Cacheu', 'Bolama'],
    "Guyana": ['Georgetown', 'Linden', 'New Amsterdam', 'Bartica', 'Skeldon'],
    "Haiti": ['Port-au-Prince', 'Cap-Haïtien', 'Gonaïves', 'Les Cayes', 'Jacmel'],
    "Honduras": ['Tegucigalpa', 'San Pedro Sula', 'La Ceiba', 'Choloma', 'El Progreso'],
    "Hungary": ['Budapest', 'Debrecen', 'Szeged', 'Miskolc', 'Pécs'],
    "Iceland": ['Reykjavík', 'Kópavogur', 'Hafnarfjörður', 'Akureyri', 'Garðabær'],
    "India": ['Mumbai', 'Delhi', 'Bengaluru', 'Hyderabad', 'Chennai'],
    "Indonesia": ['Jakarta', 'Surabaya', 'Medan', 'Bandung', 'Bekasi'],
    "Iran": ['Tehran', 'Mashhad', 'Isfahan', 'Karaj', 'Tabriz'],
    "Iraq": ['Baghdad', 'Basra', 'Mosul', 'Erbil', 'Najaf'],
    "Ireland": ['Dublin', 'Cork', 'Limerick', 'Galway', 'Waterford'],
    "Israel": ['Jerusalem', 'Tel Aviv', 'Haifa', 'Rishon LeZion', 'Petah Tikva'],
    "Italy": ['Rome', 'Milan', 'Naples', 'Turin', 'Palermo'],
    "Ivory Coast": ['Abidjan', 'Bouaké', 'Daloa', 'Yamoussoukro', 'San-Pédro'],
    "Jamaica": ['Kingston', 'Spanish Town', 'Montego Bay', 'Mandeville', 'May Pen'],
    "Japan": ['Tokyo', 'Yokohama', 'Osaka', 'Nagoya', 'Sapporo', 'Kobe', 'Kyoto', 'Fukuoka', 'Kawasaki', 'Hiroshima', 'Sendai', 'Kitakyushu', 'Chiba', 'Sakai', 'Niigata', 'Hamamatsu', 'Okayama', 'Sagamihara', 'Kumamoto', 'Shizuoka'],
    "Jordan": ['Amman', 'Zarqa', 'Irbid', 'Aqaba', 'Madaba'],
    "Kazakhstan": ['Almaty', 'Nur-Sultan', 'Shymkent', 'Karagandy', 'Aktobe'],
    "Kenya": ['Nairobi', 'Mombasa', 'Kisumu', 'Nakuru', 'Eldoret'], "Kiribati": ['South Tarawa', 'Betio Town', 'Bairiki Village', 'Bikenibeu Village', 'Teaoraereke Village'], "Kuwait": ['Kuwait City', 'Al Ahmadi', 'Hawalli', 'As Salimiyah', 'Sabah as Salim'], "Kyrgyzstan": ['Bishkek', 'Osh', 'Jalal-Abad', 'Karakol', 'Tokmok'], "Laos": ['Vientiane', 'Savannakhet', 'Pakse', 'Thakhek', 'Luang Prabang'], "Latvia": ['Riga', 'Daugavpils', 'Liepāja', 'Jelgava', 'Jūrmala'], "Lebanon": ['Beirut', 'Tripoli', 'Sidon', 'Tyre', 'Jounieh'], "Lesotho": ['Maseru', 'Teyateyaneng', 'Mafeteng', 'Hlotse', 'Mohales Hoek'], "Liberia": ['Monrovia', 'Gbarnga', 'Bensonville', 'Harper', 'Kakata'], "Libya": ['Tripoli', 'Benghazi', 'Misrata', 'Tarhuna', 'Al Khums'], "Liechtenstein": ['Vaduz', 'Schaan', 'Triesen', 'Balzers', 'Eschen'], "Lithuania": ['Vilnius', 'Kaunas', 'Klaipėda', 'Šiauliai', 'Panevėžys'], "Luxembourg": ['Luxembourg City', 'Esch-sur-Alzette', 'Dudelange', 'Schifflange', 'Bettembourg'], "Madagascar": ['Antananarivo', 'Toamasina', 'Antsirabe', 'Fianarantsoa', 'Mahajanga'],
    "Malawi": ['Lilongwe', 'Blantyre', 'Mzuzu', 'Zomba', 'Kasungu'],
    "Malaysia": ['Kuala Lumpur', 'Johor Bahru', 'George Town', 'Ipoh', 'Kuching'],
    "Maldives": ['Male', 'Hithadhoo', 'Fuvahmulah', 'Thinadhoo', 'Naifaru'],
    "Mali": ['Bamako', 'Sikasso', 'Mopti', 'Segou', 'Kayes'],
    "Malta": ['Valletta', 'Birkirkara', 'Mosta', 'Qormi', 'Zabbar'],
    "Marshall Islands": ['Majuro', 'Ebeye', 'Arno Atoll', 'Jabat Island', 'Kili Island'],
    "Mauritania": ['Nouakchott', 'Nouadhibou', 'Kaédi', 'Rosso', 'Zouérat'],
    "Mauritius": ['Port Louis', 'Beau Bassin-Rose Hill', 'Vacoas-Phoenix', 'Curepipe', 'Quatre Bornes'],
    "Mexico": ['Mexico City', 'Guadalajara', 'Monterrey', 'Puebla', 'Tijuana'],
    "Micronesia": ['Palikir', 'Weno', 'Kolonia', 'Toynim', 'Nukuoro'],
    "Moldova": ['Chisinau', 'Tiraspol', 'Balti', 'Bender', 'Ribnita'],
    "Monaco": ['Monaco-Ville', 'Monte Carlo', 'La Condamine', 'Fontvieille', 'Moneghetti'],
    "Mongolia": ['Ulaanbaatar', 'Erdenet', 'Darkhan', 'Choibalsan', 'Khovd'],
    "Montenegro": ['Podgorica', 'Nikšić', 'Herceg Novi', 'Pljevlja', 'Budva'],
    "Morocco": ['Casablanca', 'Rabat', 'Fez', 'Marrakesh', 'Agadir'],
    "Mozambique": ['Maputo', 'Matola', 'Beira', 'Nampula', 'Chimoio'],
    "Myanmar": ['Naypyidaw', 'Yangon', 'Mandalay', 'Mawlamyine', 'Taunggyi'],
    "Namibia": ['Windhoek', 'Rundu', 'Walvis Bay', 'Oshakati', 'Swakopmund'],
    "Nauru": ['Yaren', 'Baiti', 'Anabar', 'Uaboe', 'Anibare'],
    "Nepal": ['Kathmandu', 'Pokhara', 'Patan', 'Biratnagar', 'Bharatpur'],
    "Netherlands": ['Amsterdam', 'Rotterdam', 'The Hague', 'Utrecht', 'Eindhoven'],
    "New Zealand": ['Auckland', 'Wellington', 'Christchurch', 'Hamilton', 'Tauranga'],
    "Nicaragua": ['Managua', 'León', 'Masaya', 'Matagalpa', 'Chinandega'],
    "Niger": ['Niamey', 'Zinder', 'Maradi', 'Agadez', 'Tahoua'],
    "Nigeria": ['Lagos', 'Kano', 'Ibadan', 'Abuja', 'Port Harcourt'],
    "North Korea": ['Pyongyang', 'Hamhung', 'Chongjin', 'Nampo', 'Wonsan'],
    "North Macedonia": ['Skopje', 'Bitola', 'Kumanovo', 'Prilep', 'Tetovo'],
    "Norway": ['Oslo', 'Bergen', 'Trondheim', 'Stavanger', 'Drammen'],
    "Oman": ['Muscat', 'Salalah', 'Sohar', 'Nizwa', 'Sur'],
    "Pakistan": ['Islamabad', 'Karachi', 'Lahore', 'Faisalabad', 'Rawalpindi'],
    "Palau": ['Ngerulmud', 'Koror', 'Melekeok', 'Ngaramton', 'Ulimang'],
    "Palestine": ['Ramallah', 'Gaza City', 'Hebron', 'Nablus', 'Bethlehem'],
    "Panama": ['Panama City', 'Colón', 'David', 'Santiago', 'Chitré'],
    "Papua New Guinea": ['Port Moresby', 'Lae', 'Arawa', 'Mount Hagen', 'Popondetta'],
    "Paraguay": ['Asunción', 'Ciudad del Este', 'San Lorenzo', 'Luque', 'Capiatá'],
    "Peru": ['Lima', 'Arequipa', 'Trujillo', 'Chiclayo', 'Iquitos'],
    "Philippines": ['Manila', 'Quezon City', 'Davao City', 'Caloocan', 'Cebu City'],
    "Poland": ['Warsaw', 'Kraków', 'Łódź', 'Wrocław', 'Poznań'],
    "Portugal": ['Lisbon', 'Porto', 'Amadora', 'Braga', 'Setúbal'],
    "Qatar": ['Doha', 'Al Rayyan', 'Umm Salal', 'Al Wakrah', 'Al Khor'],
    "Romania": ['Bucharest', 'Cluj-Napoca', 'Timișoara', 'Iași', 'Constanța'],
    "Russia": ['Moscow', 'Saint Petersburg', 'Novosibirsk', 'Yekaterinburg', 'Nizhny Novgorod'],
    "Rwanda": ['Kigali', 'Butare', 'Gitarama', 'Ruhengeri', 'Gisenyi'],
    "Saint Kitts and Nevis": ['Basseterre', 'Charlestown', 'Cayon', 'Dieppe Bay Town', 'Old Road Town'],
    "Saint Lucia": ['Castries', 'Vieux Fort', 'Micoud', 'Soufrière', 'Gros Islet'],
    "Saint Vincent and the Grenadines": ['Kingstown', 'Georgetown', 'Byera Village', 'Barrouallie', 'Port Elizabeth'],
    "Samoa": ['Apia', 'Vaitele', 'Faleula', 'Siusega', 'Malie'],
    "San Marino": ['City of San Marino', 'Borgo Maggiore', 'Domagnano', 'Serravalle', 'Acquaviva'],
    "Sao Tome and Principe": ['São Tomé', 'Trindade', 'Neves', 'Santana', 'Guadalupe'],
    "Saudi Arabia": ['Riyadh', 'Jeddah', 'Mecca', 'Medina', 'Dammam'],
    "Senegal": ['Dakar', 'Grand Dakar', 'Thiès', 'Saint-Louis', 'Ziguinchor'],
    "Serbia": ['Belgrade', 'Novi Sad', 'Niš', 'Kragujevac', 'Subotica'],
    "Seychelles": ['Victoria', 'Anse Boileau', 'Beau Vallon', 'Cascade', 'Anse Royale'],
    "Sierra Leone": ['Freetown', 'Bo', 'Kenema', 'Koidu', 'Makeni'],
    "Slovakia": ['Bratislava', 'Košice', 'Prešov', 'Nitra', 'Žilina'],
    "Slovenia": ['Ljubljana', 'Maribor', 'Celje', 'Kranj', 'Velenje'],
    "Solomon Islands": ['Honiara', 'Auki', 'Gizo', 'Buala', 'Kirakira'],
    "Somalia": ['Mogadishu', 'Hargeisa', 'Bosaso', 'Garoowe', 'Kismayo'],
    "South Africa": ['Johannesburg', 'Cape Town', 'Durban', 'Pretoria', 'Port Elizabeth'],
    "South Korea": ['Seoul', 'Busan', 'Incheon', 'Daegu', 'Daejeon'],
    "South Sudan": ['Juba', 'Wau', 'Malakal', 'Bor', 'Aweil'],
    "Spain": ['Madrid', 'Barcelona', 'Valencia', 'Seville', 'Zaragoza'],
    "Sri Lanka": ['Colombo', 'Gampaha', 'Kandy', 'Jaffna', 'Negombo'],
    "Sudan": ['Khartoum', 'Omdurman', 'Nyala', 'Port Sudan', 'Kassala'],
    "Suriname": ['Paramaribo', 'Lelydorp', 'Nieuw Nickerie', 'Moengo', 'Nieuw Amsterdam'],
    "Swaziland": ['Mbabane', 'Manzini', 'Big Bend', 'Malkerns', 'Mhlume'],
    "Sweden": ['Stockholm', 'Gothenburg', 'Malmö', 'Uppsala', 'Västerås'],
    "Switzerland": ['Zürich', 'Geneva', 'Basel', 'Lausanne', 'Bern'],
    "Syria": ['Damascus', 'Aleppo', 'Homs', 'Latakia', 'Hama'],
    "Taiwan": ['Taipei', 'Kaohsiung', 'Taichung', 'Tainan', 'Banqiao'],
    "Tajikistan": ['Dushanbe', 'Khujand', 'Kulob', 'Qurghonteppa', 'Istaravshan'],
    "Tanzania": ['Dar es Salaam', 'Mwanza', 'Dodoma', 'Zanzibar City', 'Arusha'],
    "Thailand": ['Bangkok', 'Chiang Mai', 'Phuket', 'Nakhon Ratchasima', 'Hat Yai'],
    "Timor-Leste": ['Dili', 'Maliana', 'Suai', 'Aileu', 'Baucau'],
    "Togo": ['Lomé', 'Sokodé', 'Kara', 'Kpalimé', 'Atakpamé'],
    "Tonga": ['Nukuʻalofa', 'Neiafu', 'Haveluloto', 'Vaini', 'Pangai'],
    "Trinidad and Tobago": ['Port of Spain', 'Chaguanas', 'San Fernando', 'Arima', 'Marabella'],
    "Tunisia": ['Tunis', 'Sfax', 'Sousse', 'Kairouan', 'Bizerte'],
    "Turkey": ['Istanbul', 'Ankara', 'Izmir', 'Bursa', 'Adana'],
    "Singapore": ['Singapore'],
    "Turkmenistan": ['Ashgabat', 'Turkmenabat', 'Daşoguz', 'Mary', 'Balkanabat'],
    "Tuvalu": ['Funafuti', 'Alapi Village', 'Fakai Fou Village', 'Senala Village', 'Kulia Village'],
    "Uganda": ['Kampala', 'Gulu', 'Lira', 'Mbarara', 'Jinja'],
    "Ukraine": ['Kyiv', 'Kharkiv', 'Odesa', 'Dnipro', 'Donetsk'],
    "United Arab Emirates": ['Dubai', 'Abu Dhabi', 'Sharjah', 'Al Ain', 'Ajman'],
    "United Kingdom": ['London', 'Birmingham', 'Glasgow', 'Liverpool', 'Manchester'],
    "USA": ['New York City', 'Los Angeles', 'Chicago', 'Houston', 'Phoenix'],
    "Uruguay": ['Montevideo', 'Salto', 'Ciudad de la Costa', 'Paysandú', 'Las Piedras'],
    "Uzbekistan": ['Tashkent', 'Namangan', 'Samarkand', 'Andijan', 'Bukhara'],
    "Vanuatu": ['Port Vila', 'Luganville', 'Norsup', 'Sola', 'Lakatoro'],
    "Vatican City (Holy See)": ['Vatican City'],
    "Venezuela": ['Caracas', 'Maracaibo', 'Valencia', 'Barquisimeto', 'Maracay'],
    "Vietnam": ['Ho Chi Minh City', 'Hanoi', 'Haiphong', 'Da Nang', 'Nha Trang'],
    "Yemen": ['Sanaa', 'Taiz', 'Al Hudaydah', 'Aden', 'Ibb'],
    "Zambia": ['Lusaka', 'Kitwe', 'Ndola', 'Kabwe', 'Chingola'],
    "Zimbabwe": ['Harare', 'Bulawayo', 'Chitungwiza', 'Mutare', 'Gweru'],
    "Taiwan": ['Taipei', 'Kaohsiung', 'Taichung', 'Tainan', 'Hsinchu'],
    "Hong Kong": ['Hong Kong Island', 'Kowloon', 'New Territories', 'Lantau Island', 'Tsuen Wan'],
    "Mesopotamia": ['Mesopotamia'],
    "Palestine": ['Jerusalem', 'Gaza', 'Hebron', 'Nablus']
};

// Get references to the country and city dropdowns
const countryDropdown = document.querySelector('#country');
const cityDropdown = document.querySelector('#city');

// Add an event listener to the country dropdown
countryDropdown.addEventListener('change', () => {
    // Retrieve the selected country
    const selectedCountry = countryDropdown.value;

    // Retrieve the corresponding cities
    const cities = citiesByCountry[selectedCountry];

    // Clear the city dropdown
    cityDropdown.innerHTML = '<option value="">Stadt auswählen</option>';

    // Populate the city dropdown with the retrieved cities
    cities.forEach(city => {
        const option = document.createElement('option');
        option.value = city;
        option.textContent = city;
        cityDropdown.appendChild(option);
    });
});

}

function matchPassword() {
    var password1 = document.getElementById('password');
    var password2 = document.getElementById('password2');
    var submitButton = document.querySelector('button[type="submit"]');

    // Check if passwords match
    if (password1.value != password2.value) {
        password2.setCustomValidity('Die Passwörter müssen übereinstimmen.');
        submitButton.disabled = true;
        return;
    } else {
        password2.setCustomValidity('');
    }



    // Check if password contains at least one uppercase letter
    if (!/[A-Z]/.value(password1.value)) {
        password1.setCustomValidity('Das Passwort muss mindestens einen Großbuchstaben enthalten.');
        submitButton.disabled = true;
        return;
    } else {
        password1.setCustomValidity('');
    }

    // Check if password is at least 8 characters long
    if (password1.value.length < 8) {
        password1.setCustomValidity('Das Passwort muss mindestens 8 Zeichen lang sein.');
        submitButton.disabled = true;
        return;
    } else {
        password1.setCustomValidity('');
    }



    // Check if password contains at least one lowercase letter
    if (!/[a-z]/.value(password1.value)) {
        password1.setCustomValidity('Das Passwort muss mindestens einen Kleinbuchstaben enthalten.');
        submitButton.disabled = true;
        return;
    } else {
        password1.setCustomValidity('');
    }

    // Check if password contains at least one number
    if (!/\d/.value(password1.value)) {
        password1.setCustomValidity('Das Passwort muss mindestens eine Zahl enthalten.');
        submitButton.disabled = true;
        return;
    } else {
        password1.setCustomValidity('');
    }


    // Check if password contains at least one special character
    if (!/[ !@@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/g.value(password1.value)) {
        password1.setCustomValidity('Das Passwort muss mindestens ein Sonderzeichen enthalten.');
        submitButton.disabled = true;
        return;
    } else {
        password1.setCustomValidity('');
    }

    // Enable submit button if all checks pass
    submitButton.disabled = false;
}



function validateEmail() {
    function checkEmail() {
        var emailInput = document.getElementById('email');
        var submitButton = document.querySelector('button[type="submit"]');
        var emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        
        // Check if email is valid
        if (!emailInput.value.match(emailPattern)) {
            emailInput.setCustomValidity('Geben sie eine gültige Email ein.');
            submitButton.disabled = true;
            return;
        } else {
            emailInput.setCustomValidity('');
        }

        // Enable submit button if email is valid
        submitButton.disabled = false;
    }

    document.getElementById('email').addEventListener('input', checkEmail);
}



document.addEventListener('DOMContentLoaded', populateCitiesByCountry);
document.addEventListener('DOMContentLoaded', matchPassword);
document.addEventListener('DOMContentLoaded', validateEmail);


