using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Utility
{
    public class CodeTimer
    {

        // VARIABLES
        private long v_startTime = 0;
        private long v_endTime = 0;


        // PROPERTIES
        public float getTotalTimeInMilliseconds() {return (v_endTime - v_startTime);}

        public float getTotalTimeInSeconds() {return ((v_endTime - v_startTime) / 1000);}

        public float getTotalTimeInMinutes() {return (getTotalTimeInSeconds() / 60);}


        // FUNCTIONS
        public void startTimer() {v_startTime = System.Environment.TickCount64;}

        public void endTimer() {v_endTime = System.Environment.TickCount64;}


        public void clearTimer()
        {
            v_endTime = 0;
            v_startTime = 0;
        }
    }
}
