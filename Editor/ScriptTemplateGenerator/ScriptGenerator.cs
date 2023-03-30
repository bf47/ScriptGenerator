using UnityEngine;
using UnityEditor;
using System.IO;

namespace Bf46
{
    public class ScriptGenerator : EditorWindow
    {
        //バージョン(3/30/20:45更新)
        static string scriptgenerator_version = "0.3.3";

        /// <Summary>
        /// 親となるクラスを指定
        /// </Summary>
        private string parent_class = null;
        //private MonoScript parent_script = null;

        /// <Summary>
        /// スクリプトを保存するディレクトリを指定
        /// </Summary>
        private DefaultAsset directory_select = null;

        /// <Summary>
        /// 生成するスクリプトの名前
        /// </Summary>
        private string script_name = "Sample";

        /// <Summary>
        /// エラーがあるとtrueになる
        /// </Summary>
        private bool error = false;

        //モード検知用フラグ
        private int mode_flg = 0;
        //0:継承モード
        //1:Monoテンプレートモード



        /// <Summary>
        /// ウィンドウを表示します。
        /// </Summary>
        [MenuItem("ScriptGenerator/ScriptGenerator")]
        static void Open()
        {
            var _window = GetWindow<ScriptGenerator>();

            //タイトル
            _window.titleContent = new GUIContent("ScriptGenerator" + " v" + scriptgenerator_version);
        }
        /// <Summary>
        /// ウィンドウのパーツを表示。
        /// </Summary>
        void OnGUI()
        {
            //生成したスクリプトを保存するフォルダの指定
            directory_select = EditorGUILayout.ObjectField("保存先(ディレクトリの指定)", directory_select, typeof(DefaultAsset), true) as DefaultAsset;

            //生成するスクリプト名前の指定
            script_name = EditorGUILayout.TextField("名前(○○.cs)", script_name);

            //継承モード
            using (var _group_1 = new EditorGUILayout.ToggleGroupScope("継承モード", mode_flg == 0))
            {
                if (_group_1.enabled)
                {
                    //モードフラグ切り替え
                    mode_flg = 0;
                }

                parent_class = EditorGUILayout.TextField("親にするクラスを指定", parent_class);

            }

            //区切り線
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

            //テンプレート生成モード
            using (var _group_2 = new EditorGUILayout.ToggleGroupScope("テンプレート生成モード", mode_flg == 1))
            {
                if (_group_2.enabled)
                {
                    //モードフラグ切り替え
                    mode_flg = 1;
                }
            }

            ErrorCheck();

            EditorGUILayout.BeginVertical();
            //押すと実行       
            if (GUILayout.Button("実行(Run)"))
            {
                if (!error)
                {
                    Create();

                }
                else
                {
                    Debug.Log("エラーを解消してください");
                }
            }

            GUILayout.Space(10);

            //読み込み用(多量に生成したいときは手動でリロードをかけたほうが楽なため)
            EditorGUILayout.LabelField("生成したcsファイルが表示されない場合は押してください");
            if (GUILayout.Button("読み込み(リロードが入ります)"))
            {
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndVertical();


        }

        //エラーチェック
        void ErrorCheck()
        {
            //警告表示欄
            string _error_message = "準備完了！";
            MessageType _type = MessageType.Info;
            error = false;
            //ディレクトリが指定されていません
            if (directory_select == null)
            {
                _error_message = "ディレクトリが指定されていません";
                _type = MessageType.Error;
                error = true;
            }
            //名前が空白の場合
            else if (string.IsNullOrEmpty(script_name) || string.IsNullOrWhiteSpace(script_name))
            {
                _error_message = "名前が入力されていません";
                _type = MessageType.Error;
                error = true;
            }
            //親クラスが空白の場合
            else if (string.IsNullOrEmpty(parent_class) && mode_flg != 1 ||
                     string.IsNullOrWhiteSpace(parent_class) && mode_flg != 1)
            {
                _error_message = "親クラスが入力されていません";
                _type = MessageType.Error;
                error = true;
            }

            EditorGUILayout.HelpBox(_error_message, _type);


            //ディレクトリ以外のものをはじく
            if (directory_select != null)
            {
                // DefaultAssetのパスを取得する
                string path = AssetDatabase.GetAssetPath(directory_select);
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                // ディレクトリでなければ、指定を解除する
                bool isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);
                if (isDirectory == false)
                {
                    directory_select = null;
                }
            }
        }

        private void Create()
        {
            string _filePath1 = GetDirectoryPath() + "/" + script_name + ".cs";
            var _code = "none";
            //継承モード
            if (mode_flg == 0)
            {
                _code = template_inheritance.Replace(@"#CLASS_NAME#", script_name).Replace(@"#PARENT_NAME#", parent_class);

            }
            //テンプレート生成モード
            else if (mode_flg == 1)
            {
                _code = template_templategenerator.Replace(@"#CLASS_NAME#", script_name);
            }

            // ファイルを開く(上書き)
            StreamWriter writer = new StreamWriter(_filePath1, false);

            // 書き込み
            writer.WriteLine(_code);

            // ファイルを閉じる(忘れるな！)
            writer.Close();

            Debug.Log(_filePath1 + " が生成されました");

        }

        private string GetDirectoryPath()
        {
            if (directory_select == null)
            {
                return null;
            }
            return AssetDatabase.GetAssetPath(directory_select);
        }




        //継承モード
        public static readonly string template_inheritance = @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//親でSerializeFieldしたものが表示されます
#if UNITY_EDITOR
[CustomEditor(typeof(#PARENT_NAME#))]
#endif

public class #CLASS_NAME# : #PARENT_NAME#
{
  
}
";

        //テンプレート生成モード
        public static readonly string template_templategenerator = @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class #CLASS_NAME# : MonoBehaviour
{
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
";
    }
}