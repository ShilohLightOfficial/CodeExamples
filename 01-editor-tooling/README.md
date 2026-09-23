MapEditorModulesSO + PrefabThumbnailGenerator

Thumbnail generator for my game's level editor module palette. GenerateThumbnails renders each buildable module through an isolated camera into a transparent PNG, then forces the sprite import settings so the output is build-ready without me touching it by hand every time a module changes.

The GUID's exists because module prefabs get renamed and reordered over time, and I needed an ID that didn't depend on either.

This is an option on the ContextMenu on the ScriptableObject so I can create the module thumbnails on demand in editor.
