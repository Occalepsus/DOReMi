# DoRéMi (Des Ondes en Réalité Mixte)

Le projet DoRéMi (Des ondes en Réalité Mixe) et un projet Cassiopée 2023 de [Télécom Sudparis](https://www.telecom-sudparis.eu/), mené par
[Louise Schnee](https://github.com/Sherlousch) et [Julien Joly](https://github.com/Occalepsus).
L'objectif de ce projet est de proposer à des enseignants-chercheurs en physique un support de travaux pratiques pour des cours sur les réseaux WiFi. L'application DoRéMi tourne sur le casque Oculus Quest 2 en réalité mixte, construite avec [Unity](https://unity.com/fr) et le SDK [Oculus Integrations](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022).

## Voir les ondes en réalité Mixte

Cette application permet de voir la puissance des réseaux WiFi environnants, les sauvergarder, les comparer à un modèle en réalité mixte. Elle contient les fonctionnalités suivantes :
- Mesurer la puissances des réseaux WiFi environnants en se déplacant dans l'espace
- Voir les niveaux de puissances mesurés pour chaque réseau WiFi
- Réinitialiser les mesures pour en effectuer de nouvelles
- Sauvegarder les niveaux de puissances pour un réseau et afficher les niveaux sauvegardés
- Placer une antenne virtuelle pour calculer une modèlisation à partir de l'équation des télécommunications de Friis ([page Wikipédia](https://fr.wikipedia.org/wiki/%C3%89quation_des_t%C3%A9l%C3%A9communications))
- Voir les niveaux de puissances calculés à partir du modèle
- Adapter les paramètres du modèle aux mesures effectuées

## Cas d'utilisation

L'application DoRéMi peut avoir différents cas d'utilisation. Voici celui que nous avons imaginé et qui fait le tour des fonctionnalités de l'application :

1. Lorsque l'application DoRéMi est lancée, les mesures sont toutes vides (signalées par des points blancs) et forment un quadrillage. Commencez par regarder votre main, vous verrez en haut le réseau sélectionné, vous pouvez choisir celui que vous voulez, vous pouvez ensuite vous déplacer, et voir des points bleus reliés par des barres dont la hauteur correspond au niveau du signal WiFi sélectionné. Les points scannés peuvent aussi être gris ce qui signifie que le réseau sélectionné n'est pas détecté à l'endroit et au moment de la mesure. Sur l'interface, dans le panneau `Display mode` vous pouvez afficher les mesures que vous venez d'effectuer.

![Lancement de l'application](https://github.com/Occalepsus/DoReMi-Oculus/tree/main/Assets/DoReMi/screenshots/1-Interface%20et%20premières%20mesures.png)

2. Sortez du casque et lancez un point d'accès WiFi, par exemple sur votre téléphone. Revenez dans le casque, dans le panneau `Measure Mode` appuyez sur `B` pour réinitialiser les mesures. Cherchez ensuite dans la liste votre point d'accès et lisez le niveau dans la panneau `Display Mode`.

![Mesure d'un point d'accès](https://github.com/Occalepsus/DoReMi-Oculus/tree/main/Assets/DoReMi/screenshots/2-Mesure%20d'u%20point%20d'accès.png)

3. Dans le panneau `Measure Mode`, appuyez sur `A` après que vous ayez sélectionné votre point d'accès dans la liste pour sauvegarder les mesures que vous avez effectuées. Ces mesures sauvegardées s'afficheront par des points verts. Vous pouvez maintenant appuyer sur `B` pour réinitialiser les mesures et voir ces points verts sauvergardés, et voir dans le panneau `Display Mode` la valeur de la mesure sauvegardée.

![Sauvegarder les mesures](https://github.com/Occalepsus/DoReMi-Oculus/tree/main/Assets/DoReMi/screenshots/3-Sauvegarde%20des%20mesures.png)

4. Pour comparer vos mesures au modèle de Friis, déplacer vous pour avoir votre manette au dessus du point d'accès, que vous avez sélectionné. Vous verrez un point rouge au sol qui montre sur quel point est votre manette. Vous pouvez appuyer sur `A` dans le panneau `Config Mode` pour positionner l'antenne virtuelle au niveau de votre antenne physique. Vous devrez voir un point jaune au sol, à l'endroit où est l'antenne virtuelle. Enfin, les paramètres par défaut mis dans l'équation des télécommunications sont `Pe = -20dBm`, `Ge = 0dBi` et `Gr = 0dBi`, alors si vous voulez adapter les paramètres du modèle aux mesures vous pouvez positionner votre manette au dessus de l'antenne virtuelle, et dans le panneau `Display Mode` appuyer sur `A` pour cocher la case. Les paramètres du modèle se sont maintenant adaptés aux mesures.

![Comparaison du modèle aux mesures](https://github.com/Occalepsus/DoReMi-Oculus/tree/main/Assets/DoReMi/screenshots/4-Comparaison%20du%20modèle%20aux%20mesures.png)

## Installation

L'application est très facile à installer, il suffit de suivre les étapes suivantes :

1. Télécharger SideQuest en version avancée : https://sidequestvr.com/setup-howto
2. Télécharger l'APK de la version la plus récente de l'application DoRéMi : https://github.com/Occalepsus/DoReMi-Oculus/releases
3. Brancher le casque et le mettre en place dans SideQuest en suivant les instructions de SideQuest.
4. Cliquer sur le bouton `Install APK file from folder on computer` pour mettre l'APK de DoRéMi sur votre casque.
5. Dans le casque, mettez en place votre Guardian, le plus large possible.
6. Allez sur l'App Library, cliquez sur le bouton Filtres en haut à droite, pour sélectionner les applications de source inconnues.
7. Lancez DoRéMi.

> Attention, si vous changez le Guardian il faut relancer l'application DoRéMi au risque de voir des problèmes apparaître.

## Licence et Copyright

Ce projet est sous licence MIT, les utilisateurs ont le droit de modifier et de redistribuer l'application DoRéMi.

N'hésitez pas à contribuer au projet en l'améliorant ou en rapportant des bugs : https://github.com/Occalepsus/DoReMi-Oculus/issues. Votre aide est la bienvenue !