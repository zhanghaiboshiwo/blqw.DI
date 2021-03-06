﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace blqw.DI
{
    // 预处理日志
    class PrestoreLogger : ILogger, IDisposable
    {
        // 将处理日志的动作全部缓存起来
        private ConcurrentQueue<Func<ILogger, IDisposable>> _actions = new ConcurrentQueue<Func<ILogger, IDisposable>>();
        // 实际的日志组件
        private ILogger _logger;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_logger == null)
            {
                _actions.Enqueue(logger =>
                {
                    if (logger.IsEnabled(logLevel))
                    {
                        logger.Log(logLevel, eventId, state, exception, formatter);
                    }
                    return null;
                });
            }
            else
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => _logger?.IsEnabled(logLevel) ?? true;

        public IDisposable BeginScope<TState>(TState state)
        {
            if (_logger == null)
            {
                _actions.Enqueue(logger => logger.BeginScope(state));
                return this;
            }
            else
            {
                return _logger.BeginScope(state);
            }
        }

        /// <summary>
        /// 将缓存日志写入到指定的日志组件
        /// </summary>
        /// <param name="logger"></param>
        internal void WriteTo(ILogger logger)
        {
            if (Interlocked.CompareExchange(ref _logger, logger, null) != null)
            {
                return;
            }
            if (_actions.Count > 0)
            {
                var stack = new Stack<IDisposable>();
                while (_actions.TryDequeue(out var action))
                {
                    if (action == null) // null 表示 Dispose
                    {
                        if (stack.Count > 0)
                        {
                            stack.Pop()?.Dispose();
                        }
                    }
                    else
                    {
                        var dis = action(logger);
                        if (dis != null)
                        {
                            stack.Push(dis);
                        }
                    }
                }
            }
        }

        public void Dispose() => _actions.Enqueue(null);
    }
}
