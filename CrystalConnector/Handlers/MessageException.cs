﻿namespace CrystalConnector.Handlers;

public class MessageException : Exception
{
    public MessageException(string message) 
        : base(message)
    {
    }
}