using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CricketExt.util {
    /// <summary>
    /// Collection of ROI related constants used by the ScoreAnalyzer.
    /// </summary>
    internal static class ROIConsts {
        //Top left x, y, width, height of ROIs.
        //"Overs" text used as scoreboard checker
        public const int CHECK_1_X = 960;
        public const int CHECK_1_Y = 990;
        public const int CHECK_1_W = 60;
        public const int CHECK_1_H = 30;

        //Batter 1 name
        public const int BAT_1_X = 205;
        public const int BAT_1_Y = 945;
        public const int BAT_1_W = 245;
        public const int BAT_1_H = 28;

        //Batter 2 name
        public const int BAT_2_X = 545;
        public const int BAT_2_Y = 945;
        public const int BAT_2_W = 245;
        public const int BAT_2_H = 28;

        //Batter 1 marker
        public const int BAT_1_MARK_X = 192;
        public const int BAT_1_MARK_Y = 952;
        public const int BAT_1_MARK_W = 16;
        public const int BAT_1_MARK_H = 16;

        //Batter 2 marker 
        public const int BAT_2_MARK_X = 530;
        public const int BAT_2_MARK_Y = 952;
        public const int BAT_2_MARK_W = 16;
        public const int BAT_2_MARK_H = 16;

        public const int BAT_1_NUM = 206;
        public const int BAT_2_NUM = 256;

        //Bowler name
        public const int BOW_X = 1255;
        public const int BOW_Y = 945;
        public const int BOW_W = 245;
        public const int BOW_H = 28;

        //Offense team name
        public const int TEAM_X = 895;
        public const int TEAM_Y = 934;
        public const int TEAM_W = 60;
        public const int TEAM_H = 28;

        //Score
        public const int SCORE_X = 960;
        public const int SCORE_Y = 934;
        public const int SCORE_W = 65;
        public const int SCORE_H = 28;

        //Out
        public const int OUT_X = 920;
        public const int OUT_Y = 990;
        public const int OUT_W = 16;
        public const int OUT_H = 30;

        //Balls
        public const int BALL_X = 940;
        public const int BALL_Y = 990;
        public const int BALL_W = 16;
        public const int BALL_H = 30;

        //Out.Balls
        public const int OVER_X = 920;
        public const int OVER_Y = 990;
        public const int OVER_W = 46;
        public const int OVER_H = 30;

        //Runs
        public const int RUN_1_X = 1410;
        public const int RUN_1_Y = 988;
        public const int RUN_1_W = 16;
        public const int RUN_1_H = 16;

        public const int RUN_2_X = 1445;
        public const int RUN_2_Y = 988;
        public const int RUN_2_W = 16;
        public const int RUN_2_H = 16;

        public const int RUN_3_X = 1480;
        public const int RUN_3_Y = 988;
        public const int RUN_3_W = 16;
        public const int RUN_3_H = 16;

        public const int RUN_4_X = 1515;
        public const int RUN_4_Y = 988;
        public const int RUN_4_W = 16;
        public const int RUN_4_H = 16;

        public const int RUN_5_X = 1410;
        public const int RUN_5_Y = 988;
        public const int RUN_5_W = 16;
        public const int RUN_5_H = 16;
    }
}
