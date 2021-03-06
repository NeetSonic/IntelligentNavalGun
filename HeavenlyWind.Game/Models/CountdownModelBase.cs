﻿using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class CountdownModelBase : ModelBase, IDisposable
    {
        static IConnectableObservable<long> r_Interval;
        static IDisposable r_IntervalSubscription;

        DateTimeOffset? r_TimeToComplete;
        public DateTimeOffset? TimeToComplete
        {
            get { return r_TimeToComplete; }
            protected set
            {
                if (r_TimeToComplete != value)
                {
                    r_TimeToComplete = value;
                    IsNotificated = false;
                    OnPropertyChanged(nameof(TimeToComplete));
                }
            }
        }
        TimeSpan? r_RemainingTime;
        public TimeSpan? RemainingTime
        {
            get { return r_RemainingTime; }
            protected set
            {
                if (r_RemainingTime != value)
                {
                    r_RemainingTime = value;
                    OnPropertyChanged(nameof(RemainingTime));
                }
            }
        }

        public virtual TimeSpan RemainingTimeToNotify => TimeSpan.Zero;
        public bool IsNotificated { get; protected set; }

        static CountdownModelBase()
        {
            r_Interval = Observable.Interval(TimeSpan.FromSeconds(1.0)).Publish();
            r_Interval.Connect();
        }
        public CountdownModelBase()
        {
            r_IntervalSubscription = r_Interval.Subscribe(_ => OnTickCore());
        }

        public void Dispose()
        {
            if (r_IntervalSubscription != null)
            {
                r_IntervalSubscription.Dispose();
                r_IntervalSubscription = null;
            }
        }

        void OnTickCore()
        {
            if (!TimeToComplete.HasValue)
                RemainingTime = null;
            else
            {
                var rRemainingTime = TimeToComplete.Value - DateTimeOffset.Now;
                if (rRemainingTime.Ticks < 0L)
                    rRemainingTime = TimeSpan.Zero;

                RemainingTime = rRemainingTime;

                OnTick();

                if (rRemainingTime <= RemainingTimeToNotify && !IsNotificated)
                {
                    TimeOut();
                    IsNotificated = true;
                }
            }
        }

        protected virtual void OnTick() { }

        protected abstract void TimeOut();
    }

}
