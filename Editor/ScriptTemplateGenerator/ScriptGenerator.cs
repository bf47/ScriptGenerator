using UnityEngine;
using UnityEditor;
using System.IO;

namespace Bf46
{
    public class ScriptGenerator : EditorWindow
    {
        //�o�[�W����(3/30/20:45�X�V)
        static string scriptgenerator_version = "0.3.3";

        /// <Summary>
        /// �e�ƂȂ�N���X���w��
        /// </Summary>
        private string parent_class = null;
        //private MonoScript parent_script = null;

        /// <Summary>
        /// �X�N���v�g��ۑ�����f�B���N�g�����w��
        /// </Summary>
        private DefaultAsset directory_select = null;

        /// <Summary>
        /// ��������X�N���v�g�̖��O
        /// </Summary>
        private string script_name = "Sample";

        /// <Summary>
        /// �G���[�������true�ɂȂ�
        /// </Summary>
        private bool error = false;

        //���[�h���m�p�t���O
        private int mode_flg = 0;
        //0:�p�����[�h
        //1:Mono�e���v���[�g���[�h



        /// <Summary>
        /// �E�B���h�E��\�����܂��B
        /// </Summary>
        [MenuItem("ScriptGenerator/ScriptGenerator")]
        static void Open()
        {
            var _window = GetWindow<ScriptGenerator>();

            //�^�C�g��
            _window.titleContent = new GUIContent("ScriptGenerator" + " v" + scriptgenerator_version);
        }
        /// <Summary>
        /// �E�B���h�E�̃p�[�c��\���B
        /// </Summary>
        void OnGUI()
        {
            //���������X�N���v�g��ۑ�����t�H���_�̎w��
            directory_select = EditorGUILayout.ObjectField("�ۑ���(�f�B���N�g���̎w��)", directory_select, typeof(DefaultAsset), true) as DefaultAsset;

            //��������X�N���v�g���O�̎w��
            script_name = EditorGUILayout.TextField("���O(����.cs)", script_name);

            //�p�����[�h
            using (var _group_1 = new EditorGUILayout.ToggleGroupScope("�p�����[�h", mode_flg == 0))
            {
                if (_group_1.enabled)
                {
                    //���[�h�t���O�؂�ւ�
                    mode_flg = 0;
                }

                parent_class = EditorGUILayout.TextField("�e�ɂ���N���X���w��", parent_class);

            }

            //��؂��
            GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

            //�e���v���[�g�������[�h
            using (var _group_2 = new EditorGUILayout.ToggleGroupScope("�e���v���[�g�������[�h", mode_flg == 1))
            {
                if (_group_2.enabled)
                {
                    //���[�h�t���O�؂�ւ�
                    mode_flg = 1;
                }
            }

            ErrorCheck();

            EditorGUILayout.BeginVertical();
            //�����Ǝ��s       
            if (GUILayout.Button("���s(Run)"))
            {
                if (!error)
                {
                    Create();

                }
                else
                {
                    Debug.Log("�G���[���������Ă�������");
                }
            }

            GUILayout.Space(10);

            //�ǂݍ��ݗp(���ʂɐ����������Ƃ��͎蓮�Ń����[�h���������ق����y�Ȃ���)
            EditorGUILayout.LabelField("��������cs�t�@�C�����\������Ȃ��ꍇ�͉����Ă�������");
            if (GUILayout.Button("�ǂݍ���(�����[�h������܂�)"))
            {
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndVertical();


        }

        //�G���[�`�F�b�N
        void ErrorCheck()
        {
            //�x���\����
            string _error_message = "���������I";
            MessageType _type = MessageType.Info;
            error = false;
            //�f�B���N�g�����w�肳��Ă��܂���
            if (directory_select == null)
            {
                _error_message = "�f�B���N�g�����w�肳��Ă��܂���";
                _type = MessageType.Error;
                error = true;
            }
            //���O���󔒂̏ꍇ
            else if (string.IsNullOrEmpty(script_name) || string.IsNullOrWhiteSpace(script_name))
            {
                _error_message = "���O�����͂���Ă��܂���";
                _type = MessageType.Error;
                error = true;
            }
            //�e�N���X���󔒂̏ꍇ
            else if (string.IsNullOrEmpty(parent_class) && mode_flg != 1 ||
                     string.IsNullOrWhiteSpace(parent_class) && mode_flg != 1)
            {
                _error_message = "�e�N���X�����͂���Ă��܂���";
                _type = MessageType.Error;
                error = true;
            }

            EditorGUILayout.HelpBox(_error_message, _type);


            //�f�B���N�g���ȊO�̂��̂��͂���
            if (directory_select != null)
            {
                // DefaultAsset�̃p�X���擾����
                string path = AssetDatabase.GetAssetPath(directory_select);
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                // �f�B���N�g���łȂ���΁A�w�����������
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
            //�p�����[�h
            if (mode_flg == 0)
            {
                _code = template_inheritance.Replace(@"#CLASS_NAME#", script_name).Replace(@"#PARENT_NAME#", parent_class);

            }
            //�e���v���[�g�������[�h
            else if (mode_flg == 1)
            {
                _code = template_templategenerator.Replace(@"#CLASS_NAME#", script_name);
            }

            // �t�@�C�����J��(�㏑��)
            StreamWriter writer = new StreamWriter(_filePath1, false);

            // ��������
            writer.WriteLine(_code);

            // �t�@�C�������(�Y���ȁI)
            writer.Close();

            Debug.Log(_filePath1 + " ����������܂���");

        }

        private string GetDirectoryPath()
        {
            if (directory_select == null)
            {
                return null;
            }
            return AssetDatabase.GetAssetPath(directory_select);
        }




        //�p�����[�h
        public static readonly string template_inheritance = @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//�e��SerializeField�������̂��\������܂�
#if UNITY_EDITOR
[CustomEditor(typeof(#PARENT_NAME#))]
#endif

public class #CLASS_NAME# : #PARENT_NAME#
{
  
}
";

        //�e���v���[�g�������[�h
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