﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/********************************
 * This is part developed for TACT.Net
 * Source code available here - https://github.com/tdupont750/tact.net/blob/9d73a912dfd64bbd7fa88d3d1460c23c848af61a/framework/src/Tact/Extensions/TaskExtensions.cs
 * Author: Tom DuPont / tdupont750@gmail.com
 ********************************/
namespace RainbowDuckGames.UnityEntitySignals.Utility.Tact {
    public static class TaskExtensions {
        private const string CompleteTaskMessage = "Task must be complete";

        private const string ResultPropertyName = "Result";

        private static readonly Type GenericTaskType = typeof(Task<>);

        private static readonly ConcurrentDictionary<Type, bool> GenericTaskTypeMap =
            new ConcurrentDictionary<Type, bool>();

        public static Task IgnoreCancellation(this Task task, CancellationToken token) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            // ReSharper disable once MethodSupportsCancellation
            return task
                .ContinueWith(t => {
                    if (t.IsCanceled && token.IsCancellationRequested)
                        return Task.CompletedTask;

                    if (t.IsFaulted
                        && token.IsCancellationRequested
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.CompletedTask;

                    return t;
                })
                .Unwrap();
        }

        public static Task IgnoreCancellation(this Task task) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task
                .ContinueWith(t => {
                    if (t.IsCanceled)
                        return Task.CompletedTask;

                    if (t.IsFaulted
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.CompletedTask;

                    return t;
                })
                .Unwrap();
        }

        public static Task<T> IgnoreCancellation<T>(this Task<T> task, CancellationToken token) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            // ReSharper disable once MethodSupportsCancellation
            return task
                .ContinueWith(t => {
                    if (t.IsCanceled && token.IsCancellationRequested)
                        return GenericTask<T>.CompletedTask;

                    if (t.IsFaulted
                        && token.IsCancellationRequested
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return GenericTask<T>.CompletedTask;

                    return t;
                })
                .Unwrap();
        }

        public static Task<T> IgnoreCancellation<T>(this Task<T> task) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            return task
                .ContinueWith(t => {
                    if (t.IsCanceled)
                        return GenericTask<T>.CompletedTask;

                    if (t.IsFaulted
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return GenericTask<T>.CompletedTask;

                    return t;
                })
                .Unwrap();
        }

        public static T GetResult<T>(this Task task) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!task.IsCompleted)
                throw new ArgumentException(CompleteTaskMessage, nameof(task));

            var type = task.GetType();
            return type == typeof(Task<T>)
                ? (T) type.GetPropertyInvoker(ResultPropertyName).Invoke(task)
                : default(T);
        }

        public static object GetResult(this Task task) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (!task.IsCompleted)
                throw new ArgumentException(CompleteTaskMessage, nameof(task));

            var type = task.GetType();
            var isGenericTaskType = GenericTaskTypeMap.GetOrAdd(type,
                t => t.GetGenericTypeDefinition() == GenericTaskType);

            return isGenericTaskType
                ? type.GetPropertyInvoker(ResultPropertyName).Invoke(task)
                : null;
        }

        public static void WaitIfNeccessary(this Task task) {
            if (!task.IsCompleted || task.IsFaulted || task.IsCanceled)
                task.Wait();
        }

        private static class GenericTask<T> {
            public static readonly Task<T> CompletedTask = Task.FromResult(default(T));
        }
    }
}