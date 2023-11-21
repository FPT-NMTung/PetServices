// lazyLoadOrders.js

let page = 1; // Initial page
const pageSize = 5; // Number of orders to load per page

// Function to load more orders
const loadMoreOrders = async () => {
    // Make an AJAX request to fetch more orders
    const response = await fetch(`/Customer/MyOrders/Index?orderStatus=All&page=${page + 1}&pageSize=${pageSize}`);
    const data = await response.json();

    // Append the new orders to the existing orders container
    const ordersContainer = document.getElementById('orders-container');
    // ... logic to append new orders to the container ...

    // Increment the page number
    page++;
};

// Event listener for the "Load More" button
const loadMoreButton = document.getElementById('load-more-button');
loadMoreButton.addEventListener('click', loadMoreOrders);
