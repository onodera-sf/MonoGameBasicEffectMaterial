using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BasicEffectMaterial
{
	/// <summary>
	/// ゲームメインクラス
	/// </summary>
	public class Game1 : Game
	{
    /// <summary>
    /// パラメータの最大数
    /// </summary>
    private static int MaxParameterCount = 11;

    /// <summary>
    /// メニューリスト
    /// </summary>
    private static string[] MenuNameList = new string[]
        {
                "Alpha",
                "Diffuse (Red)",
                "Diffuse (Green)",
                "Diffuse (Blue)",
                "Emissive (Red)",
                "Emissive (Green)",
                "Emissive (Blue)",
                "Specular (Red)",
                "Specular (Green)",
                "Specular (Blue)",
                "SpecularPower"
        };


    /// <summary>
    /// グラフィックデバイス管理クラス
    /// </summary>
    private readonly GraphicsDeviceManager _graphics = null;

    /// <summary>
    /// スプライトのバッチ化クラス
    /// </summary>
    private SpriteBatch _spriteBatch = null;

    /// <summary>
    /// スプライトでテキストを描画するためのフォント
    /// </summary>
    private SpriteFont _font = null;

    /// <summary>
    /// 直前のキーボード入力の状態
    /// </summary>
    private KeyboardState _oldKeyboardState = new KeyboardState();

    /// <summary>
    /// 直前のマウスの状態
    /// </summary>
    private MouseState _oldMouseState = new MouseState();

    /// <summary>
    /// 直前のゲームパッド入力の状態
    /// </summary>
    private GamePadState _oldGamePadState = new GamePadState();

    /// <summary>
    /// モデル
    /// </summary>
    private Model _model = null;

    /// <summary>
    /// 不透明度
    /// </summary>
    private float _alpha = 1.0f;

    /// <summary>
    /// ディフーズ
    /// </summary>
    private Vector3 _diffuse = Vector3.One;

    /// <summary>
    /// エミッシブ
    /// </summary>
    private Vector3 _emissive = Vector3.Zero;

    /// <summary>
    /// スペキュラー
    /// </summary>
    private Vector3 _specular = Vector3.Zero;

    /// <summary>
    /// スペキュラーの強さ
    /// </summary>
    private float _specularPower = 5.0f;

    /// <summary>
    /// 選択しているメニューのインデックス
    /// </summary>
    private int _selectedMenuIndex = 0;


    /// <summary>
    /// パラメータテキストリスト
    /// </summary>
    private readonly string[] _parameters = new string[MaxParameterCount];


    /// <summary>
    /// GameMain コンストラクタ
    /// </summary>
    public Game1()
    {
      // グラフィックデバイス管理クラスの作成
      _graphics = new GraphicsDeviceManager(this);

      // ゲームコンテンツのルートディレクトリを設定
      Content.RootDirectory = "Content";

      // ウインドウ上でマウスのポインタを表示するようにする
      IsMouseVisible = true;
    }

    /// <summary>
    /// ゲームが始まる前の初期化処理を行うメソッド
    /// グラフィック以外のデータの読み込み、コンポーネントの初期化を行う
    /// </summary>
    protected override void Initialize()
    {
      // コンポーネントの初期化などを行います
      base.Initialize();
    }

    /// <summary>
    /// ゲームが始まるときに一回だけ呼ばれ
    /// すべてのゲームコンテンツを読み込みます
    /// </summary>
    protected override void LoadContent()
    {
      // テクスチャーを描画するためのスプライトバッチクラスを作成します
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // フォントをコンテンツパイプラインから読み込む
      _font = Content.Load<SpriteFont>("Font");

      // モデルを作成
      _model = Content.Load<Model>("Model");

      // ライトとビュー、プロジェクションはあらかじめ設定しておく
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // デフォルトのライト適用
          effect.EnableDefaultLighting();

          // ビューマトリックスをあらかじめ設定 ((0, 0, 6) から原点を見る)
          effect.View = Matrix.CreateLookAt(
              new Vector3(0.0f, 0.0f, 6.0f),
              Vector3.Zero,
              Vector3.Up
          );

          // プロジェクションマトリックスをあらかじめ設定
          effect.Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              (float)GraphicsDevice.Viewport.Width /
                  (float)GraphicsDevice.Viewport.Height,
              1.0f,
              100.0f
          );

          // モデルのマテリアルを取得 //

          // アルファ
          _alpha = effect.Alpha;

          // ディフーズ
          _diffuse = effect.DiffuseColor;

          // エミッシブ
          _emissive = effect.EmissiveColor;

          // スペキュラー
          _specular = effect.SpecularColor;

          // スペキュラーの強さ
          _specularPower = effect.SpecularPower;
        }
      }
    }

    /// <summary>
    /// ゲームが終了するときに一回だけ呼ばれ
    /// すべてのゲームコンテンツをアンロードします
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: ContentManager で管理されていないコンテンツを
      //       ここでアンロードしてください
    }

    /// <summary>
    /// 描画以外のデータ更新等の処理を行うメソッド
    /// 主に入力処理、衝突判定などの物理計算、オーディオの再生など
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Update(GameTime gameTime)
    {
      // 入力デバイスの状態取得
      KeyboardState keyboardState = Keyboard.GetState();
      MouseState mouseState = Mouse.GetState();
      GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

      // ゲームパッドの Back ボタン、またはキーボードの Esc キーを押したときにゲームを終了させます
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }


      // メニューの選択
      if ((keyboardState.IsKeyDown(Keys.Up) && _oldKeyboardState.IsKeyUp(Keys.Up)) ||
          (gamePadState.ThumbSticks.Left.Y >= 0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y < 0.5f))
      {
        // 選択メニューをひとつ上に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + _parameters.Length - 1) % _parameters.Length;
      }
      if ((keyboardState.IsKeyDown(Keys.Down) && _oldKeyboardState.IsKeyUp(Keys.Down)) ||
          (gamePadState.ThumbSticks.Left.Y <= -0.5f &&
              _oldGamePadState.ThumbSticks.Left.Y > -0.5f) ||
          (_oldMouseState.LeftButton == ButtonState.Pressed &&
           mouseState.LeftButton == ButtonState.Released))
      {
        // 選択メニューをひとつ下に移動
        _selectedMenuIndex =
            (_selectedMenuIndex + _parameters.Length + 1) % _parameters.Length;
      }

      // 各マテリアルの値を操作
      float moveValue = 0.0f;
      if (keyboardState.IsKeyDown(Keys.Left))
      {
        moveValue -= (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        moveValue += (float)gameTime.ElapsedGameTime.TotalSeconds;
      }
      if (mouseState.LeftButton == ButtonState.Pressed)
      {
        moveValue += (mouseState.X - _oldMouseState.X) * 0.005f;
      }
      if (gamePadState.IsConnected)
      {
        moveValue += gamePadState.ThumbSticks.Left.X *
                     (float)gameTime.ElapsedGameTime.TotalSeconds;
      }

      switch (_selectedMenuIndex)
      {
      case 0: // 不透明度
        _alpha = MathHelper.Clamp(_alpha + moveValue,
                                      0.0f,
                                      1.0f);
        break;
      case 1: // ディフューズ (赤)
        _diffuse.X = MathHelper.Clamp(_diffuse.X + moveValue,
                                          0.0f,
                                          1.0f);
        break;
      case 2: // ディフューズ (緑)
        _diffuse.Y = MathHelper.Clamp(_diffuse.Y + moveValue,
                                          0.0f,
                                          1.0f);
        break;
      case 3: // ディフューズ (青)
        _diffuse.Z = MathHelper.Clamp(_diffuse.Z + moveValue,
                                          0.0f,
                                          1.0f);
        break;
      case 4: // エミッシブ (赤)
        _emissive.X = MathHelper.Clamp(_emissive.X + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 5: // エミッシブ (緑)
        _emissive.Y = MathHelper.Clamp(_emissive.Y + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 6: // エミッシブ (青)
        _emissive.Z = MathHelper.Clamp(_emissive.Z + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 7: // スペキュラー (赤)
        _specular.X = MathHelper.Clamp(_specular.X + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 8: // スペキュラー (緑)
        _specular.Y = MathHelper.Clamp(_specular.Y + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 9: // スペキュラー (青)
        _specular.Z = MathHelper.Clamp(_specular.Z + moveValue,
                                           0.0f,
                                           1.0f);
        break;
      case 10: // スペキュラーの強さ
        moveValue *= 5.0f;
        _specularPower = MathHelper.Clamp(_specularPower + moveValue,
                                              0.0f,
                                              100.0f);
        break;
      }

      // マテリアルを設定
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // 不透明度
          effect.Alpha = _alpha;

          // ディフーズ
          effect.DiffuseColor = _diffuse;

          // エミッシブ
          effect.EmissiveColor = _emissive;

          // スペキュラー
          effect.SpecularColor = _specular;

          // スペキュラーの強さ
          effect.SpecularPower = _specularPower;
        }
      }

      // 入力情報を記憶
      _oldKeyboardState = keyboardState;
      _oldMouseState = mouseState;
      _oldGamePadState = gamePadState;

      // 登録された GameComponent を更新する
      base.Update(gameTime);
    }

    /// <summary>
    /// 描画処理を行うメソッド
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Draw(GameTime gameTime)
    {
      // 画面を指定した色でクリアします
      GraphicsDevice.Clear(Color.CornflowerBlue);

      // 深度バッファを有効にする
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      // モデルを描画
      foreach (ModelMesh mesh in _model.Meshes)
      {
        mesh.Draw();
      }

      // スプライトの描画準備
      _spriteBatch.Begin();

      // 操作
      _spriteBatch.DrawString(_font,
          "Up, Down : Select Menu",
          new Vector2(20.0f, 20.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "Left, right : Change Value",
          new Vector2(20.0f, 45.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "MouseClick & Drag :",
          new Vector2(20.0f, 70.0f), Color.White);
      _spriteBatch.DrawString(_font,
          "    Select Menu & Change Value",
          new Vector2(20.0f, 95.0f), Color.White);

      // 各メニュー //
      for (int i = 0; i < MenuNameList.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            MenuNameList[i],
            new Vector2(40.0f, 120.0f + i * 24.0f), Color.White);
      }

      // 各パラメータ //

      // 不透明度
      _parameters[0] = _alpha.ToString();

      // ディフューズ (赤)
      _parameters[1] = _diffuse.X.ToString();

      // ディフューズ (緑)
      _parameters[2] = _diffuse.Y.ToString();

      // ディフューズ (青)
      _parameters[3] = _diffuse.Z.ToString();

      // エミッシブ (赤)
      _parameters[4] = _emissive.X.ToString();

      // エミッシブ (緑)
      _parameters[5] = _emissive.Y.ToString();

      // エミッシブ (青)
      _parameters[6] = _emissive.Z.ToString();

      // スペキュラー (赤)
      _parameters[7] = _specular.X.ToString();

      // スペキュラー (緑)
      _parameters[8] = _specular.Y.ToString();

      // スペキュラー (青)
      _parameters[9] = _specular.Z.ToString();

      // スペキュラーの強さ
      _parameters[10] = _specularPower.ToString();

      for (int i = 0; i < _parameters.Length; i++)
      {
        _spriteBatch.DrawString(_font,
            _parameters[i],
            new Vector2(250.0f, 120.0f + i * 24.0f), Color.White);
      }

      // 選択インデックス
      _spriteBatch.DrawString(_font, "*",
          new Vector2(20.0f, 124.0f + _selectedMenuIndex * 24.0f), Color.White);

      // スプライトの一括描画
      _spriteBatch.End();

      // 登録された DrawableGameComponent を描画する
      base.Draw(gameTime);
    }
  }
}
