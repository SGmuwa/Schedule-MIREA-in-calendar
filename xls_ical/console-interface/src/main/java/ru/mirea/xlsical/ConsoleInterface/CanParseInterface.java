package ru.mirea.xlsical.ConsoleInterface;

public interface CanParseInterface <T> {
    T parse(String input) throws IllegalArgumentException;
}
