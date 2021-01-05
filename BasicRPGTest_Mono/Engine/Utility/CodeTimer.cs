using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Utility
{
    public static class CodeTimer
    {

        // VARIABLES
        private static long v_startTime = 0;
        private static long v_endTime = 0;


        // PROPERTIES
        public static float getTotalTimeInMilliseconds() {return (v_endTime - v_startTime);}

        public static float getTotalTimeInSeconds() {return ((v_endTime - v_startTime) / 1000);}

        public static float getTotalTimeInMinutes() {return (getTotalTimeInSeconds() / 60);}


        // FUNCTIONS
        public static void startTimer() {v_startTime = System.Environment.TickCount64;}

        public static void endTimer() {v_endTime = System.Environment.TickCount64;}


        public static void clearTimer()
        {
            v_endTime = 0;
            v_startTime = 0;
        }
    }
}
