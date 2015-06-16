Dwarves
=======
A game idea to mash up the hilarity of Dwarf Fortress with a nice side-on perspective, a la Terraria.

Played around with:
 * Applying Perlin and Simplex noise to generate terrain
 * Generating 3D terrain mesh via marching cubes algorithm
 * Mutation of terrain mesh when digging, allowing lighting to adjust according to nearby sources
 * Triplanar texturing via surface shader, for some smooth blending between textures from floor to wall
 * Infinite world size, as terrain chunks are loaded and offloaded as the camera approaches
 * Unity3D and XNA framworks (made a few prototypes)
