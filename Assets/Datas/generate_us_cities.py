import pyphen
import json


print("ffff")

# Liste simplifiée des 1000 plus grandes villes des États-Unis
# (en réalité, il faudrait les obtenir d'une source, ici une petite sélection d'exemple)
villes_us = [
    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
    "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte", "San Francisco", 
    "Indianapolis", "Seattle", "Denver", "Washington", "Boston", "El Paso", "Nashville", "Detroit", 
    "Oklahoma City", "Portland", "Las Vegas", "Memphis", "Louisville", "Baltimore", "Milwaukee", "Albuquerque", 
    "Tucson", "Fresno", "Sacramento", "Mesa", "Kansas City", "Atlanta", "Long Beach", "Colorado Springs", 
    "Raleigh", "Miami", "Oakland", "Minneapolis", "Tulsa", "Bakersfield", "Wichita", "Cleveland", 
    "Arlington", "Tampa", "New Orleans", "Honolulu", "Anaheim", "St. Louis", "Chula Vista", "Tallahassee",
    "Gilbert", "San Bernardino", "Boise", "Birmingham", "Spokane", "Rochester", "Modesto", "Des Moines",
    "Shreveport", "Moreno Valley", "Jackson", "Elk Grove", "Pembroke Pines", "Salem", "Eugene", "Rockford",
    "Birmingham", "Carrollton", "Clearwater", "West Valley City", "Columbia", "Odessa", "Denton", "Victorville",
    "Costa Mesa", "Miami Gardens", "Manchester", "Murrieta", "Green Bay", "Chesapeake", "Cape Coral", "Huntsville"
]

# Initialisation du découpeur de syllabes
dic = pyphen.Pyphen(lang='en_US')

# Fonction de découpage en syllabes
def syllabify(name):
    # Enlever les espaces avant de découper
    name = name.replace(' ', '')
    syllables = dic.inserted(name)
    return ".".join(syllables.split('-'))

# Création des données avec découpage
villes_syllabes = [{"ville": ville, "syllabes": syllabify(ville)} for ville in villes_us]

# Sauvegarde du fichier JSON
file_path_us = "D:/_dev/repos/Unity/UnityCourse - PCG/Assets/Datas/villes_us_syllabes.json"
with open(file_path_us, "w", encoding="utf-8") as f:
    json.dump(villes_syllabes, f, ensure_ascii=False, indent=2)

file_path_us  # Retourner le chemin du fichier généré
