using Quartz;
using Quartz.Impl;
using UpArazzi2.Tasks.Jobs;

namespace UpArazzi2.Tasks.Triggers
{
    public class YetkiSuresiTrigger
    {
        public static void Baslat()
        {
            IScheduler t = StdSchedulerFactory.GetDefaultScheduler();

            if (!t.IsStarted)
            {
                t.Start();
            }

            IJobDetail gorev = JobBuilder.Create<YetkiSuresiJob>().Build();

            ICronTrigger tetikleyici = (ICronTrigger)TriggerBuilder.Create().WithIdentity("YetkiSuresiJob", "null").WithCronSchedule("0 0 21 * * ? *").Build();

            t.ScheduleJob(gorev, tetikleyici);
        }
    }
}