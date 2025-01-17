﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class TResources
{
    public class ConstPath
    {
        public const string S_PlayerCharacter = "Character/Player/";
        public const string S_GameCharacter = "Character/Game/";

        public const string S_ChunkRender = "Chunk/Render/";
        public const string S_ChunkTile = "Chunk/Tile/";
        public const string S_ChunkTileEditor = "Chunk/Tile/Editor";
        public const string S_ChunkData = "Chunk/Data";

        public const string S_PlayerWeapon = "WeaponModel/";
        public const string S_SFXEffects = "SFX_Effects/";
        public const string S_SFXWeapon = "Weapon/";
        public const string S_InteractPortal = "Interact/Portal_";
        public const string S_InteractActionChest = "Interact/ActionChest_";
        public const string S_InteractCommon = "Interact/Interact_";

        public const string S_PETex_Noise = "Texture/PE_Noise";
        public const string S_PETex_Holograph = "Texture/PE_Holograph";

        public const string S_UI_Atlas_Numeric = "UI/Atlas/Atlas_Numeric";
        public const string S_UI_Atlas_Common = "UI/Atlas/Atlas_Common";
        public const string S_UI_Atlas_Character = "UI/Atlas/Atlas_Character";
        public const string S_UI_Atlas_InGame = "UI/Atlas/Atlas_InGame";
        public const string S_UI_Atlas_Expires = "UI/Atlas/Atlas_Expires";
        public const string S_UI_Atlas_Weapon = "UI/Atlas/Atlas_Weapon";
        public const string S_UI_Manager = "UI/UIManager";

        public const string S_Audio_GameBGM = "Audio/Background/Game_";
        public const string S_Audio_CampBGM = "Audio/Background/Camp_";
        public const string S_GameAudio_VFX = "Audio/GameVFX/";
        public const string S_UIAudio_VFX = "Audio/UIVFX/";
    }

    public static readonly Texture m_NoiseTex = Load<Texture>(ConstPath.S_PETex_Noise);
    public static readonly Texture m_HolographTex = Load<Texture>(ConstPath.S_PETex_Holograph);

    #region UI
    public static GameObject InstantiateUIManager() => Instantiate(ConstPath.S_UI_Manager);
    public static AtlasLoader GetUIAtlas_Numeric() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Numeric));
    public static AtlasLoader GetUIAtlas_Common() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Common));
    public static AtlasLoader GetUIAtlas_Character() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Character));
    public static AtlasLoader GetUIAtlas_InGame() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_InGame));
    public static AtlasLoader GetUIAtlas_Expires() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Expires));
    public static AtlasLoader GetUIAtlas_Weapon() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Weapon));
    #endregion
    #region GamePrefab
    public static GameRenderData[] GetRenderData(enum_BattleStyle levelStype) => LoadAll<GameRenderData>(ConstPath.S_ChunkRender + levelStype);
    public static LevelChunkData GetChunkData(string name) => Load<LevelChunkData>(ConstPath.S_ChunkData + "/" + name);
    public static LevelChunkData[] GetChunkDatas() => LoadAll<LevelChunkData>(ConstPath.S_ChunkData);
    public static TileItemBase[] GetChunkTiles(enum_BattleStyle _levelStyle) => LoadAll<TileItemBase>(ConstPath.S_ChunkTile + _levelStyle);
    public static TileItemBase[] GetChunkEditorTiles() => LoadAll<TileItemBase>(ConstPath.S_ChunkTileEditor);

    public static SFXDamageBase GetDamageSource(int index) => Load<SFXDamageBase>(ConstPath.S_SFXWeapon + index.ToString());

    public static SFXBase[] GetAllEffectSFX() => LoadAll<SFXBase>(ConstPath.S_SFXEffects);

    public static EntityCharacterPlayer GetPlayerCharacter(enum_PlayerCharacter character) => Load<EntityCharacterPlayer>(ConstPath.S_PlayerCharacter + (int)character);
    public static EntityCharacterBattle GetGameCharacter(int index) => Load<EntityCharacterBattle>(ConstPath.S_GameCharacter + index);
    public static WeaponBase GetPlayerWeapon(enum_PlayerWeaponIdentity weapon) => Load<WeaponBase>(ConstPath.S_PlayerWeapon + (int)weapon);
    public static InteractBattleBase GetInteract(enum_Interaction type) => Load<InteractBattleBase>(ConstPath.S_InteractCommon + type);
    #endregion
    #region Audio
    public static AudioClip GetGameBGM(enum_BattleMusic music) => Load<AudioClip>(ConstPath.S_Audio_GameBGM + music);
    public static AudioClip GetGameBGM_Styled(enum_BattleMusic music, enum_BattleStyle style) => Load<AudioClip>(ConstPath.S_Audio_GameBGM + style + "_" + music);
    public static AudioClip GetCampBGM(enum_CampMusic music) => Load<AudioClip>(ConstPath.S_Audio_CampBGM + music);
    public static AudioClip GetGameClip(enum_BattleVFX vfx) => Load<AudioClip>(ConstPath.S_GameAudio_VFX + vfx.ToString());
    #endregion

}