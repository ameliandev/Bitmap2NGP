# Bitmap2NGP
Bitmap to Neo Geo Pocket convert a tiled image to compatible Neo Geo Pocket .c file.

### Minimum requisites
​    · Microsoft .Net Framework 4.7.2 or higher

---
**Usage**: bitmap2ngp.exe [OPTIONS] FILEPATH PLANE RGBCOLOR1 [RGBCOLOR2] [RGBCOLOR3]

**Examples**:

```bash
   bitmap2ngp.exe -i ./tiledImage.png P2 68,68,68 126,13,31 255,0,255 
   bitmap2ngp.exe ./tiledImage.png PS 68,68,68 100,145,255
   bitmap2ngp.exe -ipd ./tiledImage.png P1 68,68,68 
```

**Valid image formats**:
 PNG, PCX, GIF

**Generate code file**:

 Options

         -I              generates a piece of code to install the generated
                         tile in NGP Memory with InstallTileSetAt method.
         -P              generates a piece of code to install the selected
                         pallete for the seletec plane. Method InstallTilePallete.
         -D              generates a piece of code ready to call it and.
                         display the tile in the selected plane

Required parameter

         FILEPATH        give de complete path of the tiled image.
         PLANE           sets the destination plane of the console. Possible options
                         are PS = SPRITE_PLANE, P2 = SCR_2_PLANE, P1 = SCR_1_PLANE.
         RGBCOLOR1       a minimun required color. Sets the R G B color without
                         Alpha channel. Format => R,G,B. Example 128,2,88.

Optional params

```bash
	[RGBCOLOR2]     2nd RGB Color
	[RGBCOLOR3]     3rd RGB Color
```

