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

// Setup all buttons
function setupButtons() {
    // Calendar navigation buttons
    var prevBtn = document.querySelector('.prev-btn');
    var nextBtn = document.querySelector('.next-btn');

    if (prevBtn) {
        prevBtn.onclick = function () {
            if (currentMonth === 0) {
                currentMonth = 11;
                currentYear--;
            } else {
                currentMonth--;
            }
            createCalendar();
        };
    }

    if (nextBtn) {
        nextBtn.onclick = function () {
            if (currentMonth === 11) {
                currentMonth = 0;
                currentYear++;
            } else {
                currentMonth++;
            }
            createCalendar();
        };
    }

    // Time slots
    var timeButtons = document.querySelectorAll('.time-slot');
    for (var i = 0; i < timeButtons.length; i++) {
        timeButtons[i].onclick = function () {
            selectTime(this);
        };
    }

    // Guest buttons
    var adultPlus = document.querySelector('.plus-btn[data-type="adult"]');
    var adultMinus = document.querySelector('.minus-btn[data-type="adult"]');
    var childPlus = document.querySelector('.plus-btn[data-type="child"]');
    var childMinus = document.querySelector('.minus-btn[data-type="child"]');
    var concessionPlus = document.querySelector('.plus-btn[data-type="concession"]');
    var concessionMinus = document.querySelector('.minus-btn[data-type="concession"]');

    if (adultPlus) adultPlus.onclick = function () { changeAdults(1); };
    if (adultMinus) adultMinus.onclick = function () { changeAdults(-1); };
    if (childPlus) childPlus.onclick = function () { changeChildren(1); };
    if (childMinus) childMinus.onclick = function () { changeChildren(-1); };
    if (concessionPlus) concessionPlus.onclick = function () { changeConcessions(1); };
    if (concessionMinus) concessionMinus.onclick = function () { changeConcessions(-1); };

    // Buy buttons
    var dayBuy = document.querySelector('.buy-btn[data-ticket="day"]');
    var unlimitedBuy = document.querySelector('.buy-btn[data-ticket="unlimited"]');

    if (dayBuy) dayBuy.onclick = function () { buyDay(); };
    if (unlimitedBuy) unlimitedBuy.onclick = function () { buyUnlimited(); };

    // Cart button
    var cartBtn = document.getElementById('cart-btn');
    if (cartBtn) cartBtn.onclick = function () { showCart(); };
}

// Select time
function selectTime(element) {
    var selected = document.querySelector('.time-slot.selected');
    if (selected) {
        selected.classList.remove('selected');
    }

    element.classList.add('selected');
    selectedTime = element.textContent;
    console.log('Selected time:', selectedTime);
}

// Change guest counts
function changeAdults(change) {
    adults = Math.max(0, adults + change);
    document.getElementById('adult-count').textContent = adults;
    updatePrices();
}

function changeChildren(change) {
    children = Math.max(0, children + change);
    document.getElementById('child-count').textContent = children;
    updatePrices();
}

function changeConcessions(change) {
    concessions = Math.max(0, concessions + change);
    document.getElementById('concession-count').textContent = concessions;
    updatePrices();
}

// Update prices
function updatePrices() {
    var dayTotal = (adults * dayAdult) + (children * dayChild) + (concessions * dayConcession);
    var unlimitedTotal = (adults * unlimitedAdult) + (children * unlimitedChild) + (concessions * unlimitedConcession);

    var dayPrice = document.getElementById('day-price');
    var unlimitedPrice = document.getElementById('unlimited-price');

    if (adults + children + concessions > 0) {
        if (dayPrice) dayPrice.textContent = '$' + dayTotal.toFixed(2);
        if (unlimitedPrice) unlimitedPrice.textContent = '$' + unlimitedTotal.toFixed(2);
    } else {
        if (dayPrice) dayPrice.textContent = '$-';
        if (unlimitedPrice) unlimitedPrice.textContent = '$-';
    }
}

// Buy day ticket
function buyDay() {
    if (!selectedDate) {
        alert('Please select a date');
        return;
    }
    if (!selectedTime) {
        alert('Please select a time');
        return;
    }
    if (adults + children + concessions === 0) {
        alert('Please select guests');
        return;
    }

    var price = (adults * dayAdult) + (children * dayChild) + (concessions * dayConcession);

    cart.push({
        type: 'Day Ticket',
        date: selectedDate,
        time: selectedTime,
        adults: adults,
        children: children,
        concessions: concessions,
        price: price
    });

    updateCart();
    alert('Day Ticket added! $' + price.toFixed(2));
}

// Buy unlimited pass
function buyUnlimited() {
    if (!selectedDate) {
        alert('Please select a date');
        return;
    }
    if (!selectedTime) {
        alert('Please select a time');
        return;
    }
    if (adults + children + concessions === 0) {
        alert('Please select guests');
        return;
    }

    var price = (adults * unlimitedAdult) + (children * unlimitedChild) + (concessions * unlimitedConcession);

    cart.push({
        type: 'Unlimited Pass',
        date: selectedDate,
        time: selectedTime,
        adults: adults,
        children: children,
        concessions: concessions,
        price: price
    });

    updateCart();
    alert('Unlimited Pass added! $' + price.toFixed(2));
}

// Update cart count
function updateCart() {
    var cartCount = document.getElementById('cart-count');
    if (cartCount) {
        cartCount.textContent = cart.length;
        cartCount.style.display = cart.length > 0 ? 'flex' : 'none';
    }
}

// Show cart
function showCart() {
    if (cart.length === 0) {
        alert('Cart is empty');
        return;
    }

    var message = 'CART:\n\n';
    var total = 0;

    for (var i = 0; i < cart.length; i++) {
        var item = cart[i];
        message += item.type + '\n';
        message += item.date + ' at ' + item.time + '\n';
        message += item.adults + ' adults, ' + item.children + ' children, ' + item.concessions + ' concessions\n';
        message += '$' + item.price.toFixed(2) + '\n\n';
        total += item.price;
    }

    message += 'TOTAL: $' + total.toFixed(2);
    alert(message);
}