//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// サウンド管理
	/// </summary>
	[AddComponentMenu("Utage/Lib/Sound/SoundManager")]
	public class SoundManager : MonoBehaviour
	{
		/// <summary>
		/// シングルトンなインスタンスの取得
		/// </summary>
		/// <returns></returns>
		public static SoundManager GetInstance()
		{
			if (null == instance)
			{
				GameObject go = new GameObject("SoundManager");
				instance = go.AddComponent<SoundManager>();
			}
			return instance;
		}
		static SoundManager instance;

		const string GameObjectNameSe = "One shot audio";

		/// <summary>
		/// サウンドのタイプ（SE以外）
		/// </summary>
		public enum StreamType
		{
			/// <summary>BGM</summary>
			Bgm,
			/// <summary>環境音</summary>
			Ambience,
			/// <summary>ボイス</summary>
			Voice,
			/// <summary>タイプの数</summary>
			Max,
		};
		SoundStreamFade[] streamTbl = new SoundStreamFade[(int)StreamType.Max];	//BGN等のストリーム
		List<AudioSource> curretFrameSeList = new List<AudioSource>();	//今のフレームで鳴らしたSEのリスト

		/// <summary>
		/// マスターボリューム
		/// </summary>
		public float MasterVolume
		{
			get
			{
				return this.masterVolume;
			}
			set
			{
				if (Mathf.Approximately(masterVolume, value)) return;

				masterVolume = value;
				for (int i = 0; i < (int)StreamType.Max; i++)
				{
					RefleashMasterVolume((StreamType)i);
				}
				RefleashSeVolume();
			}
		}
		float masterVolume = 1.0f;

		/// <summary>
		/// ボリュームの取得
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <returns>ボリューム(0.0～1.0)</returns>
		public float GetVolume(StreamType type) { return masterVolume * masterVolumeTbl[(int)type]; }

		/// <summary>
		/// マスターボリュームの設定
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <param name="volume">ボリューム(0.0～1.0)</param>
		public void SetMasterVolume(StreamType type, float volume)
		{
			if (Mathf.Approximately(masterVolumeTbl[(int)type], volume)) return;

			masterVolumeTbl[(int)type] = volume;
			RefleashMasterVolume(type);
		}
		float[] masterVolumeTbl = new float[(int)StreamType.Max];

		/// <summary>
		/// 音声再生中はBGMを少し小さく鳴らすための補正値
		/// </summary>
		public float BgmVolumeFilterOfPlayingVoice
		{
			get { return this.bgmVolumeFilterOfPlayingVoice; }
			set
			{
				this.bgmVolumeFilterOfPlayingVoice = value;
				RefleashMasterVolume(StreamType.Bgm);
			}
		}
		float bgmVolumeFilterOfPlayingVoice;

		void RefleashMasterVolume(StreamType type)
		{
			if (type == StreamType.Bgm && !IsStop(StreamType.Voice))
			{
				streamTbl[(int)type].SetMasterVolume(GetVolume(type) * bgmVolumeFilterOfPlayingVoice);
			}
			else
			{
				streamTbl[(int)type].SetMasterVolume(GetVolume(type));
			}
		}

		/// <summary>
		/// SEのマスターボリューム
		/// </summary>
		public float MasterVolumeSe
		{
			set
			{
				if (Mathf.Approximately(masterVolumeSe, value)) return;

				masterVolumeSe = value;
				RefleashSeVolume();
			}
		}
		float masterVolumeSe = 1.0f;
		//SEのボリューム
		float GetVolumeSe() { return masterVolume * masterVolumeSe; }


		/// <summary>
		/// 指定の音が再生中か
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <param name="clip">オーディオクリップ</param>
		/// <returns>再生中ならtrue。そうでないならfalse</returns>
		public bool IsPlaying(StreamType type, AudioClip clip)
		{
			return streamTbl[(int)type].IsPlaying(clip);
		}

		[SerializeField]
		float defaultFadeTime = 0.2f;
		[SerializeField]
		float defaultVoiceFadeTime = 0.05f;
		[SerializeField]
		float defaultVolume = 1.0f;

		/// <summary>
		/// サウンドの再生
		/// </summary>
		/// <param name="type">サウンドのタイプ</param>
		/// <param name="file">サウンドファイル</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="isReplay">直前が同じサウンドなら鳴らしなおすか</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public void Play(StreamType type, AssetFile file, bool isLoop, bool isReplay)
		{
			float fadeTime = type == StreamType.Voice ? defaultVoiceFadeTime : defaultFadeTime;
			Play(type, file, isLoop, fadeTime, isReplay);
		}

		/// <summary>
		/// サウンドの再生
		/// </summary>
		/// <param name="type">サウンドのタイプ</param>
		/// <param name="file">サウンドファイル</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="fadeTime">フェード時間</param>
		/// <param name="isReplay">直前が同じサウンドなら鳴らしなおすか</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public void Play( StreamType type, AssetFile file, bool isLoop, float fadeTime, bool isReplay)
		{
			if (!isReplay && IsPlaying(type, file.Sound))
			{
			}
			else
			{
				SoundStream stream = Play(type, file.Sound, fadeTime, defaultVolume, isLoop, (file.LoadFlags & AssetFileLoadFlags.Streaming) == AssetFileLoadFlags.Streaming );
				if (null != stream)
				{
					file.AddReferenceComponet(stream.gameObject);
				}
			}
		}

		/// <summary>
		/// サウンドの再生
		/// </summary>
		/// <param name="type">サウンドのタイプ</param>
		/// <param name="clip">オーディオクリップ</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="isStreaming">ストリーム再生するか</param>
		/// <param name="isReplay">直前が同じサウンドなら鳴らしなおすか</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public void Play(StreamType type, AudioClip clip, bool isLoop, bool isStreaming, bool isReplay)
		{
			float fadeTime = type == StreamType.Voice ? defaultVoiceFadeTime : defaultFadeTime;
			Play(type, clip, isLoop, isStreaming, fadeTime, isReplay);
		}

		/// <summary>
		/// サウンドの再生
		/// </summary>
		/// <param name="type">サウンドのタイプ</param>
		/// <param name="clip">オーディオクリップ</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="isStreaming">ストリーム再生するか</param>
		/// <param name="fadeTime">フェード時間</param>
		/// <param name="isReplay">直前が同じサウンドなら鳴らしなおすか</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public void Play(StreamType type, AudioClip clip, bool isLoop, bool isStreaming, float fadeTime, bool isReplay)
		{
			if (!isReplay && IsPlaying(type, clip))
			{
			}
			else
			{
				Play(type, clip, fadeTime, defaultVolume, isLoop, isStreaming);
			}
		}

		/// <summary>
		/// 再生（直前をフェードアウトしてから再生）
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <param name="clip">オーディオクリップ</param>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="volume">再生ボリューム</param>
		/// <param name="isLoop">ループ再生するか</param>
		/// <param name="isStreaming">ストリーム再生するか</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		internal SoundStream Play(StreamType type, AudioClip clip, float fadeTime, float volume, bool isLoop, bool isStreaming )
		{
			return streamTbl[(int)type].Play(clip, fadeTime, GetVolume(type), volume, isLoop, isStreaming);
		}

		/// <summary>
		/// 指定したタイプのサウンドの停止
		/// </summary>
		/// <param name="type">サウンドのタイプ</param>
		public void Stop(StreamType type)
		{
			float fadeTime = type == StreamType.Voice ? defaultVoiceFadeTime : defaultFadeTime;
			Stop(type, fadeTime);
		}

		/// <summary>
		/// フェードアウトして曲を停止
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <param name="fadeTime">フェードする時間</param>
		public void Stop(StreamType type, float fadeTime)
		{
			streamTbl[(int)type].Stop(fadeTime);
		}

		/// <summary>
		/// 指定のサウンドが停止しているか
		/// </summary>
		/// <param name="type">タイプ</param>
		/// <returns>鳴っていなければtrue、鳴っていればfalse</returns>
		public bool IsStop(StreamType type)
		{
			return streamTbl[(int)type].IsStop();
		}

		/// <summary>
		/// フェードアウトして曲全てを停止
		/// </summary>
		/// <param name="fadeTime">フェードアウトの時間</param>
		public void StopAll(float fadeTime)
		{
			foreach (var item in streamTbl)
			{
				item.Stop(fadeTime);
			}
		}

		/// <summary>
		/// SEの再生
		/// </summary>
		/// <param name="clip">オーディオクリップ</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public AudioSource PlaySE(AudioClip clip)
		{
			return PlaySE(clip, defaultVolume);
		}

		/// <summary>
		/// SEの再生
		/// </summary>
		/// <param name="file">サウンドファイル</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public AudioSource PlaySE(AssetFile file)
		{
			AudioSource audio = PlaySE(file.Sound, defaultVolume);
			if (audio)
			{
				file.AddReferenceComponet (audio.gameObject);
			}
			return audio;
		}

		/// <summary>
		/// SE再生
		/// </summary>
		/// <param name="clip">オーディオクリップ</param>
		/// <param name="volume">再生ボリューム</param>
		/// <returns>再生をしているサウンドストリーム</returns>
		public AudioSource PlaySE(AudioClip clip, float volume)
		{
			float vol = volume * GetVolumeSe();

			//音量0なので、鳴らさない
			if (vol <= 0) return null;
			//同一フレームで既に鳴っているので鳴らさない（多重再生防止）
			foreach (AudioSource audio in curretFrameSeList)
			{
				if (clip == audio.clip)
				{
					return null;
				}
			}

			AudioSource se = PlaySeClip(clip, vol);
			curretFrameSeList.Add(se);
			return se;
		}

		//オーディオクリップをSEとして再生
		AudioSource PlaySeClip(AudioClip clip, float volume)
		{
			GameObject go = UtageToolKit.AddChild(this.transform, new GameObject(GameObjectNameSe));
			AudioSource source = go.AddComponent<AudioSource>();
			source.clip = clip;
			source.volume = volume;
			source.Play();
			Destroy(go, clip.length);
			return source;
		}

		//SEを検索
		AudioSource[] FindSeArray()
		{
			AudioSource[] audioArray = this.gameObject.GetComponentsInChildren<AudioSource>();
			List<AudioSource> seList = new List<AudioSource>();
			foreach (AudioSource audio in audioArray)
			{
				if (GameObjectNameSe == audio.gameObject.name)
				{
					seList.Add(audio);
				}
			}
			return seList.ToArray();
		}

		/// <summary>
		/// セーブデータ用のバイナリ変換
		/// 再生中のBGMのファイル情報などをバイナリ化
		/// </summary>
		/// <returns>データのバイナリ</returns>
		public byte[] ToSaveDataBuffer()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//バイナリ化
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					WriteSaveData(writer);
				}
				return stream.ToArray();
			}
		}

		/// <summary>
		/// セーブデータを読みこみ
		/// </summary>
		/// <param name="buffer">セーブデータのバイナリ</param>
		public void ReadSaveDataBuffer(byte[] buffer)
		{
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					ReadSaveData(reader);
				}
			}
		}

		const int VERSION = 0;
		//セーブデータ用のバイナリ書き込み
		void WriteSaveData(BinaryWriter writer)
		{
			writer.Write(VERSION);
			//BGMと環境音のみを再生
			streamTbl[(int)StreamType.Bgm].WriteSaveData(writer);
			streamTbl[(int)StreamType.Ambience].WriteSaveData(writer);
		}
		//セーブデータ用のバイナリ読み込み
		void ReadSaveData(BinaryReader reader)
		{
			StopAll(0.1f);
			int version = reader.ReadInt32();
			if (version == VERSION)
			{
				//BGMと環境音のみを再生
				streamTbl[(int)StreamType.Bgm].ReadSaveData(reader, MasterVolume);
				streamTbl[(int)StreamType.Ambience].ReadSaveData(reader, MasterVolume);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}


		void RefleashSeVolume()
		{
			//SEは細かいボリューム調整はしないが、ボリューム0になったら、音を止めることはする
			if (masterVolume * masterVolumeSe <= 0.0f)
			{
				AudioSource[] seArray = FindSeArray();

				foreach (AudioSource se in seArray)
				{
					se.Stop();
				}
			}
		}

		void Awake()
		{
			if (null != instance)
			{
				Destroy(this.gameObject);
				return;
			}
			else
			{
				instance = this;
				for (int i = 0; i < (int)StreamType.Max; i++)
				{
					StreamType type = (StreamType)i;
					masterVolumeTbl[i] = 1.0f;
					streamTbl[i] = CreateSoundStreamFade(type.ToString());
				}
			}
		}

		void LateUpdate()
		{
			curretFrameSeList.Clear();

			//BGMは音声再生状態によってマスターボリュームが変わるのでここで処理をする
			RefleashMasterVolume(StreamType.Bgm);
		}

		SoundStreamFade CreateSoundStreamFade(string name)
		{
			GameObject go = UtageToolKit.AddChild(this.transform, new GameObject(name));
			SoundStreamFade stream = go.AddComponent<SoundStreamFade>();
			return stream;
		}
	}
}