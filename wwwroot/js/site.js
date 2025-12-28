document.addEventListener("DOMContentLoaded", function () {
    updateCartBadge();
});

function updateCartBadge() {
    fetch('/Orders/GetCartItemCount')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(count => {
            const badge = document.getElementById('cart-badge');
            if (count > 0) {
                badge.innerText = count;
                badge.classList.remove('d-none');
            } else {
                badge.classList.add('d-none');
            }
        })
        .catch(error => console.error('Error fetching cart count:', error));
}