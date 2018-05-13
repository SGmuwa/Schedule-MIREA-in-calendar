import java.io.Closeable;

// Интерфейс по работе с Excel файлами. Экземпляр такого интерфейса должен хранит в себе дескриптор файла.
public interface ExcelFileInterface extends Closeable {

    /**
     * Получение текстовых данных из файла.
     * @param Column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param Row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке.
     */
    String getCellData(int Column, int Row);

}
