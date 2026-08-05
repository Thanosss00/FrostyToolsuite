# Duplicate Game Mode Plugin

## by Nuuby

Duplicate Game Modes seen in [Frosty Editor](https://github.com/CadeEvs/FrostyToolsuite)

Supports SWBF 2015, 2017. Modifications can easily be made for PVZ, Battlefield, and other Frostbite Games

To Use:

## Step 1
Find a SubWorldData file, right-click, and select Duplicate Game Mode

![Screenshot (9)](https://github.com/user-attachments/assets/2dfd24f7-4532-4938-801e-e0287e3a488c)

## Step 2
Create a new gamemode name, path of existing gamemode, and where to create the new files
![image](https://github.com/user-attachments/assets/443cfd8d-50a4-49eb-9ba0-eb333a597514)

## Step 3
The tool will do it's work and generate new layerdata files and subworld files. Using a bundle manager will be required to setup networking, mesh variation dbs, and etc

![image](https://github.com/user-attachments/assets/d9ebb146-96be-4f21-96ae-bffb3009bafe)

## Other

There is an Import Spawns Button. To use, create a .txt file in this format:
```
AlternateSpawnEntityData, X, Y, Z, W, TeamId, Priority, DefaultEnabled
SpawnLocationFinderEntityData, X, Y, Z, Index, TeamId, UNUSED, DefaultEnabled
```

Example:
```
AlternateSpawnEntityData,-15.470,76.800,140.620,425.0,1,1,True
AlternateSpawnEntityData,-15.470,76.800,140.620,425.0,2,1,True
AlternateSpawnEntityData,-15.470,76.800,140.620,425.0,1,1,True
AlternateSpawnEntityData,-83.230,71.890,-46.190,290.1,1,1,True
SpawnLocationFinderEntityData,-112.500,0.000,-5.835,0,1,0,True
SpawnLocationFinderEntityData,-32.105,0.000,-6.565,0,1,0,True
SpawnLocationFinderEntityData,-32.934,0.000,-79.534,0,1,0,True
SpawnLocationFinderEntityData,-111.141,0.000,-82.233,0,1,0,True
SpawnLocationFinderEntityData,-20.751,0.000,158.131,1,1,0,True
SpawnLocationFinderEntityData,-10.131,0.000,158.099,1,1,0,True
SpawnLocationFinderEntityData,-10.397,0.000,105.857,1,1,0,True
SpawnLocationFinderEntityData,-20.799,0.000,105.450,1,1,0,True
```
This might seem tedious but using a to-be-released Unity Tool, you can generate these in seconds

Credit:
[MagixGames](https://github.com/MagixGames) - Example Code and Help when needed 
