// Simple variables
var selectedDate = '';
var selectedTime = '';
var adults = 0;
var children = 0;
var concessions = 0;
var cart = [];
var currentMonth = 9; // October (0-based)
var currentYear = 2024;

// Prices
var dayAdult = 39.99;
var dayChild = 19.99;
var dayConcession = 29.99;
var unlimitedAdult = 59.98;
var unlimitedChild = 28.98;
var unlimitedConcession = 36.98;

// When page loads
window.onload = function () {
    createCalendar();
    setupButtons();
    updatePrices();
};

// Create simple calendar
function createCalendar() {
    var grid = document.querySelector('.calendar-grid');
    var monthDisplay = document.querySelector('.current-month');
    if (!grid) return;

    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

    // Update month display
    if (monthDisplay) {
        monthDisplay.textContent = months[currentMonth] + ' ' + currentYear;
    }

    // Clear existing days
    var days = grid.querySelectorAll('.calendar-day');
    for (var i = 0; i < days.length; i++) {
        days[i].remove();
    }

    // Get today's date
    var today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time to midnight for accurate comparison

    // Get days in current month
    var daysInMonth = new Date(currentYear, currentMonth + 1, 0).getDate();

    // Add days for current month
    for (var day = 1; day <= daysInMonth; day++) {
        var dayDiv = document.createElement('div');
        dayDiv.className = 'calendar-day';
        dayDiv.textContent = day;
        dayDiv.setAttribute('data-day', day);

        // Create date object for this day
        var dayDate = new Date(currentYear, currentMonth, day);
        dayDate.setHours(0, 0, 0, 0);

        // Check if this day is in the past
        var isPast = dayDate < today;

        if (isPast) {
            dayDiv.className += ' unavailable';
        } else {
            dayDiv.className += ' available';
        }

        grid.appendChild(dayDiv);
    }

    // Add click event to whole grid
    grid.onclick = function (e) {
        if (e.target.classList.contains('available')) {
            selectDate(e.target);
        }
    };
}

// Select a date
function selectDate(element) {
    var selected = document.querySelector('.calendar-day.selected');
    if (selected) {
        selected.classList.remove('selected');
    }

    element.classList.add('selected');
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    selectedDate = months[currentMonth] + ' ' + element.textContent + ', ' + currentYear;
    console.log('Selected date:', selectedDate);
}

