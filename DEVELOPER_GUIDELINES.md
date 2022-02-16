Developer Guidelines
=======

## Guidelines

To keep the game consistent, and easy to maintain, a few guidelines and conventions have been made in order to make the game easily acessable.
  
### Code Guidelines:
- The project folows the standard C# coding conventions for all C# code, and the standard godot shader naming conventions ([see here](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)).
- This project's language is C#, with GDScript only being used if absolutely necessary.
- Native script may be used in places where performance or features may be improved by it's use.

### File Naming Conventions:
- All folders should be using PascalCase, within reason (so don't go changing around the .gitignore, or your IDE's hidden folders 😅️)
- All assets, such as images, models, audio files, or blender files should be using snake_case.
- Objects, be them inside of blender, or godot, should be in pascal case, so that everything loaded into the scene is using the same casing. (This may also include audacity track names, inkscape layer names, and krita layer names, yet this isn't a big deal, as it is likely that these names won't be used inside of a scene.) 


## Software Guidelines:

This project aims to use as much Free and Open Source (FOSS) software as possible, in order to allow anyone to contribute, regardless of their device, and also to prevent license issues from arising. To do this, there are specific recommended softwares that should be used.

- Vector Images (.svg) [textures and assets] -> Inkscape
- Bitmap Images (.png) [textures and assets] -> Krita or GIMP
- 3D Models (.blend/.gltf) [models] -> Blender (project files should be saved as __.blend__, with a license attached [see asset license convention](#Licenses), should be exported as .gltf, with a copyright attached if you please in the export settings, and modifiers applied in export settings.)
- Audio (.mp3) [audio] -> Audacity (project files should be shaved as __.aup__, should be exported as __.mp3__, project.)
- Audio (other) [audio] -> Other audio software, such as musescore, can be used for creating sound, they are recommended to be open source though.
- Godot -> Curerntly Godot 3.4, yet updating the version, assuming all remains compatible, is fine.

### Licenses
If you please, you may want to attach a separate license to assets you've created, such as 3D models. A template has been created, so that licenses are easy to create. Here is an example:

should_be_the_same_as_that_of_the_asset __.txt__
```
should_be_the_same_as_that_of_the_asset.blend - Ⓒ Zekiah-A (https://github.com/Zekiah-A)

This 3D model is licensed under the GPL V3.0 license, see the license attached under the head of this repository (../../../LICENSE). Textures and additional assets used in this model may be distributed under their own respective licenses.

NOTE:
  ...
TODO:
  ...
```

## Model and Texture Guidelines:

Models and textures have no strict regulations, but it should just be noted that you should attempt to make it fit in with the general aesthetic of the game. Performance is also quite important, so it is prefered that most textures, if possible, be exported at resolutions of 1024x1024, or lower (such as 512*512, or any other number to the power of 2). However, it's fine for things like HDRIs or high fidelity textures to use higher resolutions. When you are creating a model, also make sure to consider poly count, by thinking things such as "How can I make this model as low poly as possible, while also looking good.", and also considering "If the player will never see this, then it's fine to not put any geometry here."

## Footnotes:
Once again, if you're reading this, you're probably epic, and any contribuions to this project could not be more appreciated! ❤️ 😎️
