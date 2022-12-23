using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace OJ.UX.Editor.Utility
{
    public class DateTimePickerWindow : PopupWindowContent
    {
        private enum DateTimePickerState
        {
            SingleMonth,
            SingleYear,
            YearRange,
            YearRangeRange,
            Time,
            Hour,
            Minute,
            Seconds
        }
        
        private readonly SerializedProperty _dateAsLong;

        private int _currentDisplayMonth;
        private int _currentDisplayDay;
        private int _currentDisplayYear;

        private int _currentYearRangeMin;

        private int _currentDisplayHour;
        private int _currentDisplayMinute;
        private int _currentDisplaySecond;

        private DateTimePickerState _state;
        private DateTime _currentDate;

        private const int YearRange = 12;
        private const float buttonWidth = 55;
        private const float buttonHeight = 38;
        
        public DateTimePickerWindow(SerializedProperty dateAsLong)
        {
            _dateAsLong = dateAsLong;
            _currentDate = new DateTime(_dateAsLong.longValue);

            _currentDisplayMonth = _currentDate.Month;
            _currentDisplayDay = _currentDate.Day;
            _currentDisplayYear = _currentDate.Year;

            _currentDisplayHour = _currentDate.Hour;
            _currentDisplayMinute = _currentDate.Minute;
            _currentDisplaySecond = _currentDate.Second;

            _currentYearRangeMin = _currentDisplayYear - YearRange / 2;
        }
        
        public override void OnGUI(Rect rect)
        {
            switch (_state)
            {
                case DateTimePickerState.SingleMonth:
                    DrawSingleMonthDisplay();
                    DrawTimeOption();
                    break;
                case DateTimePickerState.SingleYear:
                    DrawSingleYearDisplay();
                    DrawTimeOption();
                    break;
                case DateTimePickerState.YearRange:
                    DrawYearRangeDisplay();
                    DrawTimeOption();
                    break;
                case DateTimePickerState.YearRangeRange:
                    DrawTimeOption();
                    break;
                case DateTimePickerState.Time:
                    DrawDateOption();
                    DrawTimeDisplay();
                    break;
                case DateTimePickerState.Hour:
                    DrawDateOption();
                    DrawTimePortionDisplay(ref _currentDisplayHour, 
                        new int[] { 12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
                        HourSelectRespectAMOrPM);
                    break;
                case DateTimePickerState.Minute:
                    DrawDateOption();
                    DrawTimePortionDisplay(ref _currentDisplayMinute, new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 });
                    break;
                case DateTimePickerState.Seconds:
                    DrawDateOption();
                    DrawTimePortionDisplay(ref _currentDisplaySecond, new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 200);
        }

        private void DrawSingleMonthDisplay()
        {
            EditorGUILayout.BeginHorizontal();

            if (OJEditorUtility.Button("<<", GUI.backgroundColor, buttonWidth))
            {
                GoToNextMonth(false);
            }

            var monthAsDateTime = new DateTime(_currentDisplayYear, _currentDisplayMonth, 1);
            if (OJEditorUtility.Button(monthAsDateTime.ToString("MMM yyyy")))
            {
                _state = DateTimePickerState.SingleYear;
            }
            
            if (OJEditorUtility.Button(">>", GUI.backgroundColor, buttonWidth))
            {
                GoToNextMonth(true);
            }
            
            EditorGUILayout.EndHorizontal();

            const float dayButtonSize = 35;
            var dayLabelStyle = new GUIStyle(EditorStyles.label);
            dayLabelStyle.alignment = TextAnchor.MiddleCenter;
            dayLabelStyle.fontStyle = FontStyle.Bold;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            OJEditorUtility.Button("Su", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("Mo", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("Tu", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("We", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("Th", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("Fr", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            OJEditorUtility.Button("Sa", GUI.backgroundColor, dayButtonSize, dayLabelStyle);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            var originalColor = GUI.color;
            var fadedColor = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
            var firstDayOfMonth = new DateTime(_currentDisplayYear, _currentDisplayMonth, 1);
            var daysProcessed = -(int)firstDayOfMonth.DayOfWeek;
            
            for (var rowIndex = 0; rowIndex < 6; ++rowIndex)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                
                for (var columnIndex = 0; columnIndex < 7; ++columnIndex)
                {
                    var day = firstDayOfMonth + TimeSpan.FromDays(daysProcessed);
                    var color = IsCurrentDay(day) ? Color.blue : GUI.backgroundColor;
                    GUI.color = day.Month == firstDayOfMonth.Month ? originalColor : fadedColor;
                    if (OJEditorUtility.Button($"{day.Day:00}", color, dayButtonSize))
                    {
                        SetDay(day, false);
                    }

                    GUI.color = originalColor;
                    daysProcessed++;
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawSingleYearDisplay()
        {
            EditorGUILayout.BeginHorizontal();

            if (OJEditorUtility.Button("<<", GUI.backgroundColor, buttonWidth))
            {
                _currentDisplayYear--;
            }

            if (OJEditorUtility.Button(_currentDisplayYear.ToString(CultureInfo.InvariantCulture)))
            {
                _state = DateTimePickerState.YearRange;
            }
            
            if (OJEditorUtility.Button(">>", GUI.backgroundColor, buttonWidth))
            {
                _currentDisplayYear++;
            }
            
            EditorGUILayout.EndHorizontal();
            
            var firstDayOfYear = new DateTime(_currentDisplayYear, 1, 1);
            var monthsProcessed = 0;
            for (var rowIndex = 0; rowIndex < 3; ++rowIndex)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(buttonHeight));
                EditorGUILayout.Space();
                
                for (var columnIndex = 0; columnIndex < 4; ++columnIndex)
                {
                    var month = firstDayOfYear.AddMonths(monthsProcessed);
                    var color = IsCurrentMonth(month) ? Color.blue : GUI.backgroundColor;
                    if (OJEditorUtility.Button($"{month:MMM}", color, buttonWidth, buttonHeight))
                    {
                        _currentDisplayMonth = month.Month;
                        _state = DateTimePickerState.SingleMonth;
                    }
                    monthsProcessed++;
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawYearRangeDisplay()
        {
            EditorGUILayout.BeginHorizontal();

            if (OJEditorUtility.Button("<<", GUI.backgroundColor, buttonWidth))
            {
                _currentYearRangeMin -= 12;
            }

            OJEditorUtility.Button(_currentDisplayYear.ToString(CultureInfo.InvariantCulture));

            if (OJEditorUtility.Button(">>", GUI.backgroundColor, buttonWidth))
            {
                _currentYearRangeMin += 12;
            }
            
            EditorGUILayout.EndHorizontal();

            var firstDayOfYear = new DateTime(_currentYearRangeMin, 1, 1);
            var yearsProcessed = 0;
            
            for (var rowIndex = 0; rowIndex < 3; ++rowIndex)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(buttonHeight));
                EditorGUILayout.Space();
                
                for (var columnIndex = 0; columnIndex < 4; ++columnIndex)
                {
                    var year = firstDayOfYear.AddYears(yearsProcessed);
                    var color = IsCurrentYear(year) ? Color.blue : GUI.backgroundColor;
                    if (OJEditorUtility.Button($"{year:yyyy}", color, buttonWidth, buttonHeight))
                    {
                        _currentDisplayYear = year.Year;
                        _state = DateTimePickerState.SingleYear;
                    }
                    yearsProcessed++;
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void DrawTimeDisplay()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            DrawTimeSpinner(ref _currentDisplayHour, DateTimePickerState.Hour, GoToNextHour, FormatHour);
            DrawTimeSpinner(ref _currentDisplayMinute, DateTimePickerState.Minute, GoToNextMinute);
            DrawTimeSpinner(ref _currentDisplaySecond, DateTimePickerState.Seconds, GoToNextSecond);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(buttonWidth - 5);

            var isMorning = _currentDisplayHour < 12;
            if (OJEditorUtility.Button(isMorning? "AM" : "PM", isMorning ? Color.red : Color.blue, buttonWidth, buttonHeight))
            {
                SwapFromAMToPM();
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTimeSpinner(ref int currentDisplay, DateTimePickerState state, Action<bool> goToNextAction, Func<int, string> displayFunc = null)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(buttonWidth));
            EditorGUILayout.Space(10);
            
            if (OJEditorUtility.Button("▲", GUI.backgroundColor, buttonWidth, buttonHeight))
            {
                goToNextAction(true);
            }
            
            var display = displayFunc != null ? displayFunc(currentDisplay) : $"{currentDisplay:00}";
            if (OJEditorUtility.Button(display, GUI.backgroundColor, buttonWidth, buttonHeight))
            {
                _state = state;
            }
            
            if (OJEditorUtility.Button("▼", GUI.backgroundColor, buttonWidth, buttonHeight))
            {
                goToNextAction(false);
            }
            
            EditorGUILayout.EndVertical();
        }

        private void DrawTimePortionDisplay(ref int currentTimeOptionDisplay, int[] options, Func<int, int> conversionFunc = null)
        {
            var rowCount = 3;
            var columnCount = options.Length / rowCount;

            
            for (var row = 0; row < rowCount; ++row)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(buttonHeight));
                EditorGUILayout.Space();
                
                for (var col = 0; col < columnCount; ++col)
                {
                    var index = row * columnCount + col;
                    if (OJEditorUtility.Button($"{options[index]:00}", GUI.backgroundColor, buttonWidth, buttonHeight))
                    {
                        currentTimeOptionDisplay = conversionFunc != null ? conversionFunc(options[index]) : options[index];
                        UpdateDateValue();
                        _state = DateTimePickerState.Time;
                    }
                }
                
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawDateOption()
        {
            var isDark = EditorGUIUtility.isProSkin;
            var tex = Resources.Load<Texture>(isDark ? "calendar" : "calendar_black");
            var style = new GUIStyle(GUI.skin.button);
            style.padding = new RectOffset(0, 0, 5, 5);
            if (OJEditorUtility.CenteredButton(tex, GUI.backgroundColor, 150, 30, style))
            {
                _state = DateTimePickerState.SingleMonth;
            }
        }
        
        private void DrawTimeOption()
        {
            GUILayout.FlexibleSpace();
            var isDark = EditorGUIUtility.isProSkin;
            var stopWatchTex = Resources.Load<Texture>(isDark ? "stopwatch" : "stopwatch_black");
            var style = new GUIStyle(GUI.skin.button);
            style.padding = new RectOffset(0, 0, 5, 5);
            if (OJEditorUtility.CenteredButton(stopWatchTex, GUI.backgroundColor, 150, 30, style))
            {
                _state = DateTimePickerState.Time;
            }
        }

        private bool IsCurrentDay(DateTime date)
        {
            return _currentDate.Day == date.Day && _currentDate.Year == date.Year && _currentDate.Month == date.Month;
        }
        
        private bool IsCurrentMonth(DateTime month)
        {
            return _currentDate.Month == month.Month && _currentDate.Year == month.Year;
        }

        private bool IsCurrentYear(DateTime year)
        {
            return _currentDate.Year == year.Year;
        }

        private void SetDay(DateTime date, bool setTime)
        {
            _currentDisplayMonth = date.Month;
            _currentDisplayYear = date.Year;
            _currentDisplayDay = date.Day;

            if (setTime)
            {
                _currentDisplayHour = date.Hour;
                _currentDisplayMinute = date.Minute;
                _currentDisplaySecond = date.Second;
            }

            UpdateDateValue();
        }
        
        private void GoToNextHour(bool increase)
        {
            var dateTime = _currentDate.AddHours(increase ? 1 : -1);
            SetDay(dateTime, true);
        }
        
        private string FormatHour(int hour)
        {
            if (hour % 12 == 0)
            {
                return "12";
            }
            
            return $"{(hour % 12):00}";
        }
        
        private int HourSelectRespectAMOrPM(int hour)
        {
            if (_currentDate.Hour < 12 || hour == 12)
            {
                return hour;
            }

            return hour + 12;
        }

        private void SwapFromAMToPM()
        {
            var newDate = _currentDate.Hour < 12 ? _currentDate.AddHours(12) : _currentDate.AddHours(-12);
            SetDay(newDate, true);
        }
        
        private void GoToNextMinute(bool increase)
        {
            var dateTime = _currentDate.AddMinutes(increase ? 1 : -1);
            SetDay(dateTime, true);
        }
        
        private void GoToNextSecond(bool increase)
        {
            var dateTime = _currentDate.AddSeconds(increase ? 1 : -1);
            SetDay(dateTime, true);
        }

        private void GoToNextMonth(bool increase)
        {
            if (_currentDisplayMonth == 1 && !increase)
            {
                _currentDisplayMonth = 12;
                _currentDisplayYear--;
            }
            else if (_currentDisplayMonth == 12 && increase)
            {
                _currentDisplayMonth = 1;
                _currentDisplayYear++;
            }
            else
            {
                _currentDisplayMonth += increase ? 1 : -1;
            }
            
        }

        private void UpdateDateValue()
        {
            _currentDate = (new DateTime(_currentDisplayYear, _currentDisplayMonth,
                _currentDisplayDay, _currentDisplayHour, _currentDisplayMinute, _currentDisplaySecond));

            _currentYearRangeMin = _currentDisplayYear - YearRange / 2;
            if (_dateAsLong != null)
            {
                _dateAsLong.longValue = _currentDate.Ticks;
                _dateAsLong.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}