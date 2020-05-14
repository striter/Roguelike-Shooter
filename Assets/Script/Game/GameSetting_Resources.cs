using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class TResources
{
    public class ConstPath
    {
        public const string S_PlayerEntity = "Entity/PlayerCharacter/";
        public const string S_EnermyEntity = "Entity/AICharacter/";

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
        public const string S_UI_Atlas_InGame = "UI/Atlas/Atlas_InGame";
        public const string S_UI_Atlas_Expires = "UI/Atlas/Atlas_Expires";
        public const string S_UI_Atlas_Weapon = "UI/Atlas/Atlas_Weapon";
        public const string S_UI_Manager = "UI/UIManager";

        public const string S_Audio_GameBGM = "Audio/Background/Game_";
        public const string S_Audio_CampBGM = "Audio/Background/Camp_";
        public const string S_GameAudio_VFX = "Audio/GameVFX/";
        public const string S_UIAudio_VFX = "Audio/UIVFX/";
    }

    static Texture m_NoiseTex = null;
    public static Texture GetNoiseTex()
    {
        if (m_NoiseTex == null)
            m_NoiseTex = Load<Texture>(ConstPath.S_PETex_Noise);
        return m_NoiseTex;
    }

    #region UI
    public static GameObject InstantiateUIManager() => Instantiate(ConstPath.S_UI_Manager);
    public static AtlasLoader GetUIAtlas_Numeric() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Numeric));
    public static AtlasLoader GetUIAtlas_Common() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Common));
    public static AtlasLoader GetUIAtlas_InGame() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_InGame));
    public static AtlasLoader GetUIAtlas_Expires() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Expires));
    public static AtlasLoader GetUIAtlas_Weapon() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Weapon));
    #endregion
    #region GamePrefab
    public static GameRenderData[] GetRenderData(enum_GameStyle levelStype) => LoadAll<GameRenderData>(ConstPath.S_ChunkRender + levelStype);
    public static LevelChunkData GetChunkData(string name) => Load<LevelChunkData>(ConstPath.S_ChunkData + "/" + name);
    public static LevelChunkData[] GetChunkDatas() => LoadAll<LevelChunkData>(ConstPath.S_ChunkData);
    public static TileItemBase[] GetChunkTiles(enum_GameStyle _levelStyle) => LoadAll<TileItemBase>(ConstPath.S_ChunkTile + _levelStyle);
    public static TileItemBase[] GetChunkEditorTiles() => LoadAll<TileItemBase>(ConstPath.S_ChunkTileEditor);

    public static SFXWeaponBase GetDamageSource(int index) => Load<SFXWeaponBase>(ConstPath.S_SFXWeapon + index.ToString());

    public static SFXBase[] GetAllEffectSFX() => LoadAll<SFXBase>(ConstPath.S_SFXEffects);

    public static EntityCharacterPlayer GetPlayerCharacter(enum_PlayerCharacter character) => Load<EntityCharacterPlayer>(ConstPath.S_PlayerEntity + (int)character);
    public static EntityCharacterBase GetEnermyCharacter(int index) => Load<EntityCharacterBase>(ConstPath.S_EnermyEntity + index);
    public static WeaponBase GetPlayerWeapon(enum_PlayerWeaponIdentity weapon) => Load<WeaponBase>(ConstPath.S_PlayerWeapon + (int)weapon);
    public static InteractGameBase GetInteract(enum_Interaction type) => Load<InteractGameBase>(ConstPath.S_InteractCommon + type);
    #endregion
    #region Audio
    public static AudioClip GetGameBGM(enum_GameMusic music) => Load<AudioClip>(ConstPath.S_Audio_GameBGM + music);
    public static AudioClip GetGameBGM_Styled(enum_GameMusic music, enum_GameStyle style) => Load<AudioClip>(ConstPath.S_Audio_GameBGM + style + "_" + music);
    public static AudioClip GetCampBGM(enum_CampMusic music) => Load<AudioClip>(ConstPath.S_Audio_CampBGM + music);
    public static AudioClip GetGameClip(enum_GameVFX vfx) => Load<AudioClip>(ConstPath.S_GameAudio_VFX + vfx.ToString());
    #endregion

}