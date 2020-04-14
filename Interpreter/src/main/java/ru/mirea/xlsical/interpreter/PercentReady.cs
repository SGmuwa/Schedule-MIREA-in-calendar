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

using System;

namespace ru.mirea.xlsical.interpreter
{
    /// <summary>
    /// Класс, который служит для управления процентом готовности задачи.
    /// Данный класс является потокобезопасным.
    /// </summary>
    public class PercentReady
    {
        /// <summary>
        /// Создание нового экземпляра управления процентом готовности.
        /// Или создание потомка, который изменяет общий сумматор.
        /// </summary>
        /// <param name="whole">Ссылка на целую часть.</param>
        /// <param name="coefficient">Коэффициент взаимодействия на целую часть. Допустимые значения: от 0 до 1.</param>
        /// <param name="subscribers">Функция, которую надо вызывать при каждом изменении значения.</param>
        public PercentReady(PercentReady whole = null, float coefficient = 1.0f, params TransferValue[] subscribers)
        {
            this.whole = whole;
            foreach(TransferValue t in subscribers)
                this.subscriber += t;
            if (0.0f <= coefficient && coefficient <= 1.0f)
            {
                this.coefficient = coefficient;
                if (whole != null)
                {
                    lock (whole.sc)
                    {
                        if (whole.sumOfCoefficientInPieces + coefficient > 1.0000001f)
                            throw new ArgumentException("Сумма коэффициентов целой части зашкаливает! Должно быть от 0 до 1 включительно! А получается: " + whole.sumOfCoefficientInPieces + coefficient);
                        else
                            whole.sumOfCoefficientInPieces += coefficient;
                    }
                }
            }
            else
                throw new ArgumentException("Коэффициент должен быть от 0 до 1 включительно!");
        }
        
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

        /// <summary>
        /// Подписчики на изменения.
        /// </summary>
        private event TransferValue subscriber;

        /// <summary>
        /// Сумма коэффициентов в частях.
        /// </summary>
        private float sumOfCoefficientInPieces = 0.0f;

        /// <summary>
        /// Получает сумму занятых коэффициентов частями.
        /// Используйте для того, чтобы вычислить свободное пространство.
        /// </summary>
        /// <value>Сумма занятых коэффициентов частями.</value>
        public float FreeCoefficient
        {
            get
            {
                lock (sc)
                {
                    return sumOfCoefficientInPieces - float.MinValue + 1.0f;
                }
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
                ArgumentException error = null;
                lock (sc)
                { // Блокировка этого состояния.
                    if (sumOfCoefficientInPieces != 0.0f)
                        throw new ArgumentException("Вы потеряли доступ к изменению данного поля. Его можно получить используя его части.");
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
                        error = new ArgumentException("float ready can be only 0.0f ... 1.0f! Argument = " + value);
                }
                if (error != null)
                    throw error;
                if (whole == null)
                    if (this.subscriber != null)
                        this.subscriber?.Invoke(this);
            }
        }

        private ArgumentException sendToWhole(float ready)
        {
            ArgumentException error = null;
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
                        error = new ArgumentException("I can't send new value: " + commit);
                }
            }
            else
            { // Это корень! Это и есть сумма всех частей.
              // synchronized sc // уже заблокировано.
                if (0.0f <= ready && ready <= 1.0f)
                    this.ready = ready;
                else
                    error = new ArgumentException("While i go through wholes: float ready can be only 0.0f ... 1.0f! Argument = " + ready);
            }
            if (error == null)
                if (subscriber != null)
                    subscriber?.Invoke(this);
            return error;
        }

        public override string ToString()
            => $"{ready * 100f :N0}%";
    }
}
