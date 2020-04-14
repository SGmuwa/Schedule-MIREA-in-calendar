/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace ru.mirea.xlsical.interpreter
{

    /**
     * Класс, который служит для управления процентом готовности задачи.
     * Данный класс является потокобезопасным.
     * @since 18.11.2018
     * @version 2018-12-08
     * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
     */
    public class PercentReady
    {

        /// <summary>
        /// Создание нового экземпляра управления процентом готовности.
        /// Или создание потомка, который изменяет общий сумматор.
        /// </summary>
        /// <param name="whole">Ссылка на целую часть.</param>
        /// <param name="coefficient">Коэффициент взаимодействия на целую часть. Допустимые значения: от 0 до 1.</param>
        /// <param name="subscriber">Функция, которую надо вызывать при каждом изменении значения.</param>
        public PercentReady(PercentReady whole = null, float coefficient = 1.0f, ICanUsePercentReady subscriber = null)
        {
            this.whole = whole;
            this.subscriber = subscriber;
            if (0.0f <= coefficient && coefficient <= 1.0f)
            {
                this.coefficient = coefficient;
                if (whole != null)
                {
                    lock (whole.sc)
                    {
                        if (whole.sumOfCoefficientInPieces + coefficient > 1.0000001f)
                            throw new IllegalArgumentException("Сумма коэффициентов целой части зашкаливает! Должно быть от 0 до 1 включительно! А получается: " + whole.sumOfCoefficientInPieces + coefficient);
                        else
                            whole.sumOfCoefficientInPieces += coefficient;
                    }
                }
            }
            else
                throw new IllegalArgumentException("Коэффициент должен быть от 0 до 1 включительно!");
        }

        /**
         * На сколько готов текущий загрузчик?
         */
        /// <summary>
        /// Описывает готовность.
        /// 0 — совсем не готов.
        /// 1 — полностью готов.
        /// </summary>
        private float ready = 0.0f;

        /// <summary>
        /// Объект синхронизации.
        /// </summary>
        private readonly object sc = new object();

        /// <summary>
        /// Ссылка на родителя.
        /// </summary>
        private readonly PercentReady whole;

        /// <summary>
        /// Коэффициент влияния на родителя.
        /// </summary>
        public readonly float coefficient;
        /**
         * Подписчик на изменения.
         */
        private readonly ICanUsePercentReady subscriber;
        /**
         * Сумма коэффициентов в частях.
         */
        private float sumOfCoefficientInPieces = 0.0f;

        /**
         * Получает сумму занятых коэффициентов частями.
         * Используйте для того, чтобы вычислить свободное пространство.
         * @return Сумма занятых коэффициентов частями
         */
        public float GetFreeCoefficient()
        {
            lock (sc)
            {
                return sumOfCoefficientInPieces - Float.MIN_VALUE + 1.0f;
            }
        }

        /// <summary>
        /// Процент готовности задачи от 0 до 1.
        /// </summary>
        /// <value>Процент готовности от 0 до 1.</value>
        public float Ready
        {
            get
            {
                lock (sc)
                {
                    return ready;
                }
            }

            set
            {
                IllegalArgumentException error = null;
                lock (sc)
                { // Блокировка этого состояния.
                    if (sumOfCoefficientInPieces != 0.0f)
                        throw new IllegalAccessError("Вы потеряли доступ к изменению данного поля. Его можно получить используя его части.");
                    if (0.0f <= value && value <= 1.0f)
                    {
                        if (whole == null)
                            this.ready = value;
                        else
                        {
                            error = this.sendToWhole(value);
                        }
                    }
                    else
                        error = new IllegalArgumentException("float ready can be only 0.0f ... 1.0f! Argument = " + value);
                }
                if (error != null)
                    throw error;
                if (whole == null)
                    if (this.subscriber != null)
                        this.subscriber.transferValue(this);
            }
        }

        private IllegalArgumentException sendToWhole(float ready)
        {
            IllegalArgumentException error = null;
            if (whole != null)
            {
                lock (whole.sc)
                { // Блокировка целой части.
                  // Восстановление значения.
                    float wholeWithoutThis = whole.ready - (this.ready * coefficient);
                    this.ready = ready;
                    float commit = wholeWithoutThis + (this.ready * coefficient);
                    if (-0.1f <= commit && commit <= 1.1f)
                    {
                        if (commit > 1.0f)
                            commit = 1.0f;
                        else if (commit < 0.0f)
                            commit = 0.0f;
                        error = whole.sendToWhole(commit);
                    }
                    else
                        error = new IllegalArgumentException("I can't send new value: " + commit);
                }
            }
            else
            { // Это корень! Это и есть сумма всех частей.
              // synchronized sc // уже заблокировано.
                if (0.0f <= ready && ready <= 1.0f)
                    this.ready = ready;
                else
                    error = new IllegalArgumentException("While i go through wholes: float ready can be only 0.0f ... 1.0f! Argument = " + ready);
            }
            if (error == null)
                if (subscriber != null)
                    subscriber.transferValue(this);
            return error;
        }

        public override string toString()
            => string.Format("%3.1f%%", ready * 100f);
    }
}
