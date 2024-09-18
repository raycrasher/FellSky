using System;
using static Godot.GD;
using Godot;

namespace FellSky;

public enum LogLevel
{
    TRACE,
    DEBUG,
    INFO,
    WARNING,
    ERROR
}

public class Logger
{
    private static char[] DELIMITERS = { '[', ']' };

    private string name;
    private string klass;
    private string type;

    public Logger(Node loggee)
    {
        name = loggee.Name;
        type = loggee.GetClass();
        klass = loggee.GetType().Name;
    }

    public Logger(Type loggee)
    {
        name = "";
        type = "";
        klass = loggee.Name;
    }

    private static string GetLevelName(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.TRACE:
                return "TRACE";
            case LogLevel.DEBUG:
                return "DEBUG";
            case LogLevel.INFO:
                return "INFO";
            case LogLevel.WARNING:
                return "WARNING";
            case LogLevel.ERROR:
                return "ERROR";
            default:
                return "UNKNOWN";
        }
    }

    private string GetHeader(LogLevel level)
    {
        var level_name = GetLevelName(level);
        return $"{DELIMITERS[0]}{level_name}{DELIMITERS[1]}" +
            $"{DELIMITERS[0]}{name}@{klass}: {type}{DELIMITERS[1]}";
    }

    public void log(LogLevel level, string message)
    {
        Print($"{GetHeader(level)} {message}");
    }

    public void trace(string message)
    {
        log(LogLevel.TRACE, message);
    }

    public void debug(string message)
    {
        log(LogLevel.DEBUG, message);
    }

    public void info(string message)
    {
        log(LogLevel.INFO, message);
    }

    public void warning(string message)
    {
        log(LogLevel.WARNING, message);
    }

    public void error(string message)
    {
        log(LogLevel.ERROR, message);
    }
}