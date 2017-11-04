using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace RankingSystem
{

    public class RankingData
    {
        public int order;
        public string[] name = new string[3];
        public int score;
    }

    public class FileOperation
    {
        [SerializeField]
        private const string m_tutorialpath = "tutorial_ranking.dat";
        [SerializeField]
        private const string m_stage1path = "stage1_ranking.dat";
        [SerializeField]
        private const string m_stage2path = "stage2_ranking.dat";
        [SerializeField]
        private const string m_stage3path = "stage3_ranking.dat";
        private const int ORDER_MAX = 3;

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="_data"></param>
        public void ReadData(List<RankingData> _data, int _stageNum)
        {

             
            // ない場合生成
            if (!Directory.Exists("save/"))
            {
                Directory.CreateDirectory("save/");
            }

            var path = "save/" + GetStageRankingDataPath(_stageNum);

            bool flag = false;
            flag = File.Exists(path);

            FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryReader reader = new BinaryReader(file);

            RankingData data = new RankingData();


            // 読み込み
            if (!flag)
            {
                ZeroClear(_data,file);
                return;
            }

            for (int i = 0; i < ORDER_MAX; ++i)
            {
                data.order = reader.ReadInt32();
                data.name[0] = reader.ReadString();
                data.name[1] = reader.ReadString();
                data.name[2] = reader.ReadString();
                data.score = reader.ReadInt32();
                _data.Add(data);
            }
            reader.Close();
            file.Close();
        }

        string GetStageRankingDataPath(int _stageNum)
        {
            switch (_stageNum)
            {
                case 0:
                    return m_tutorialpath;
                case 1:
                    return m_stage1path;
                case 2:
                    return m_stage2path;
                case 3:
                    return m_stage3path;
            }
            return null;
        }

        void ZeroClear(List<RankingData> _data , FileStream _file)
        {
           
            BinaryWriter writer = new BinaryWriter(_file);

            RankingData work = new RankingData();

            for (int i = 0; i < ORDER_MAX; ++i)
            {
                work.order = (i + 1);
                work.name[0] = "------";
                work.name[1] = "------";
                work.name[2] = "------";
                work.score = 0;

                _data.Add(work);

                writer.Write(work.order);
                writer.Write(work.name[0]);
                writer.Write(work.name[1]);
                writer.Write(work.name[2]);
                writer.Write(work.score);
            }
            writer.Close();
        }

        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="_data"></param>
        /*
        public void WriteData(RankingData _data)
        {
            FileStream file = new FileStream(m_path, FileMode.Open, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(file);

            for(int i=0;i< ORDER_MAX; ++i)
            {
                writer.Write(i + 1);
                writer.Write()
            }
        }*/


        // 現在のスコアがランキングに更新されるかどうか
        // 更新されればそのスコアの順位が返ってくる（１～３）
        // ランキング外なら -１  が返ってくる
        public int GetRankingOeder(int _score, int _stageNum)
        {
            List<RankingData> readData = new List<RankingData>();

            // 1位から3位までのデータ読み取り
            ReadData(readData, _stageNum);

            for (int i = 0; i < ORDER_MAX; ++i)
            {
                if (readData[i].score <= _score)
                {
                    return i + 1;
                }
            }
            return -1;
        }


        public void WriterData(int _score , int k)
        {

        }
    }

}
