﻿using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPolly
{
    class Program
    {
        static int Compute()
        {
            var a = 0;
            return 1 / a;
        }

        static void ZeroException()
        {
            throw new DivideByZeroException();
        }

        static void Main(string[] args)
        {
            //RetryThreeTime();

            WaitRetry();

            Console.Read();
        }

        static void ReportaError(Exception e, TimeSpan tiempo, int intento, Context contexto)
        {
            Console.WriteLine($"异常: {intento:00} (调用秒数: {tiempo.Seconds} 秒)\t执行时间: {DateTime.Now}");
        }

        static void WaitRetry()
        {
            var policyaWaitAndRetry = Policy
                .Handle<DivideByZeroException>()
                .WaitAndRetry(new[] {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(7)
                }, ReportaError);

            policyaWaitAndRetry.Execute(() =>
            {
                ZeroException();
            });
        }

        static void RetryThreeTime()
        {
            try
            {
                var retryTwoTimesPolicy = Policy.Handle<DivideByZeroException>()
                    .Retry(3, (ex, count) =>
                    {
                        Console.WriteLine("执行失败! 重试次数 {0}", count);
                        Console.WriteLine("异常来自 {0}", ex.GetType().Name);
                    });

                retryTwoTimesPolicy.Execute(() =>
                {
                    Compute();
                });
            }
            catch (DivideByZeroException e)
            {
                Console.WriteLine($"Excuted Failed,Message: ({e.Message})");

            }
        }
    }
}
