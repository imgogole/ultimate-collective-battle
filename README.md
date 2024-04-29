# Ultimate Collective Battle

Ultimate Collective Battle est un jeu vidéo réalisé par I'm Gogole en 2024 et sorti la même année. Le joueur incarne son propre personnage, élève de Polytech Marseille, et de s'affronter entre amis dans l'arène Polytech afin de gagner des scores !

## Gameplay et règles

### Vue d'ensemble

(Note importante : la carte Polytech est légèrement diffente pour des raisons de gameplay)

La carte comprend deux étages. Vous apparaissez dans la salle B4 de Polytech, ou l'amphi A1, gardez la clé correspondante à votre salle et récupérez celle de vote adversaire pour accéder à sa salle. Affrontez les ennemis en utilisant des capacités uniques qui vous permettrons d'avoir des performances et de vaincre vos adversaires pour finir la partie. Le jeu se déroule en plusieurs round.

(Note : le nombre de round est actuellement infini)

Les personnages possèdent tous une compétence passive, une compétence active et une compétence ultime. Ces compétences permettent de tuer vos adversaires ou de les contrôler pour les empécher de franchir la salle adverse.

### Personnages actuels

Voici les personnages que vous pouvez actuellement jouer :

- Arnaud
- Dalil
- Idriss
- Julien
- Lou
- Mohamad
- Romain
- Samuel
- Thomas
- Titouan
- Virgil
- Yassine

L'entiereté des compétences et des statistiques des personnages sont consultables dans les jeux dans la catégorie Information.

### Runes

Voici les runes que vous pouvez utiliser pour améliorer les capacités de gameplay des personnages :

- Ingénieur informatique
- Ingénieur génie civile
- Ingénieur bio-médicale

## Gameplay

### Variables d'état

Chaque personnage peut se déplacer de gauche à droite, sauter et prendre les escaliers. Les interactions possibles peuvent se faire avec les escaliers pour les utiliser (20 secondes de délai de récupération).
Chaque personnage possède des points de vie (PV actuels / PV max), des dégâts d'attaque, de la vitesse d'attaque, de la vitesse de déplacement, de la réduction de dégâts reçus, de la portée d'attaque et des états de champions non-communs (montrés côté client dans la barre d'UI).

Chaque personnage possède aussi une taille proportionnel au PV max (égale à 0.004 * HitPointsMax + 0.6 mètres, cette valeur est comprise entre 1 et 1.2 mètre).

Utiliser l'action d'attaque si au moins un ennemi se trouve à la portée d'attaque d'un personnage frappe tous les ennemis à portée.
La valeur des dégâts infligés par défaut est égale aux dégâts d'attaque du personnage joué, cette valeur est modifiée pour chaque ennemi touché selon leur réduction de dégâts reçus. Seuls les dégâts critiques ne sont pas modifiés par la réduction de dégâts reçus.

Voici l'ordre d'execution des algortihmes d'application des dégâts :

- ANYWHERE. Effets non à l'impact (EFFECTS)
- 1. Dégâts additives (ADDITIONAL DAMAGE)
- 2. Dégâts multiplicatives (MULTIPLICATIVE DAMAGE)
- 3. Réduction aux dégâts reçus (ARMOR)
- 4. Dégâts critiques additives (ADDITIONAL TRUE DAMAGE)
- 5. Dégâts critiques multiplicatives (MULTIPLICATIVE TRUE DAMAGE)
- 6. Effets à l'impact (IMPACT EFFECTS)

### Contrôles

Les compétences utilisés par les personnages peuvent altérer les contrôles d'un joueur. Ces contrôles permettent de créer une contrainte ou un avantage pour un ou des joueurs, ennemis ou alliés dans le jeu.
Voici la liste des contrôles possibles :


 

