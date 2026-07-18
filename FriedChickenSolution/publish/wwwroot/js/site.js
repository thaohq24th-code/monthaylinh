// ============================================================
// FRIED CHICKEN - MAIN SITE.JS
// Fixed: Cart shows product image/name/price
// Added: User-to-User chat (localStorage), Avatar helper
// ============================================================

const NO_IMAGE = '/images/no-image.png.svg';

class CartManager {
    constructor() {
        // Cart data: array of { productId, name, price, image, quantity }
        // Persist across page navigations via localStorage
        this.STORAGE_KEY = 'friedchicken_cart';
        this.items = this.loadFromStorage();
        this.init();
    }

    // ========== LOCALSTORAGE PERSISTENCE ==========
    loadFromStorage() {
        try {
            const raw = localStorage.getItem(this.STORAGE_KEY);
            if (!raw) return [];
            const parsed = JSON.parse(raw);
            return Array.isArray(parsed) ? parsed : [];
        } catch (e) {
            console.warn('[Cart] Không thể đọc giỏ hàng từ localStorage:', e);
            return [];
        }
    }

    saveToStorage() {
        try {
            localStorage.setItem(this.STORAGE_KEY, JSON.stringify(this.items));
        } catch (e) {
            console.warn('[Cart] Không thể lưu giỏ hàng vào localStorage:', e);
        }
    }

    init() {
        this.setupCartUI();
        this.setupCheckout();
        this.setupProductButtons();
        this.setupFiltersAndSearch();
        this.setupFeedback();
    }

    // ========== CART UI ==========
    setupCartUI() {
        const cartTrigger = document.getElementById('cart-trigger-btn');
        const cartClose = document.getElementById('cart-close-btn');
        const cartOverlay = document.getElementById('cart-overlay');

        if (cartTrigger) cartTrigger.addEventListener('click', (e) => { e.preventDefault(); this.openCart(); });
        if (cartClose) cartClose.addEventListener('click', () => this.closeCart());
        if (cartOverlay) cartOverlay.addEventListener('click', () => this.closeCart());

        this.updateCartUI();
    }

    openCart() {
        const sidebar = document.getElementById('cart-sidebar');
        const overlay = document.getElementById('cart-overlay');
        if (sidebar) sidebar.classList.add('active');
        if (overlay) overlay.classList.add('active');
    }

    closeCart() {
        const sidebar = document.getElementById('cart-sidebar');
        const overlay = document.getElementById('cart-overlay');
        if (sidebar) sidebar.classList.remove('active');
        if (overlay) overlay.classList.remove('active');
    }

    updateCartUI() {
        const cartCount = document.getElementById('cart-count');
        const cartItems = document.getElementById('cart-items');

        const totalItems = this.items.reduce((sum, item) => sum + item.quantity, 0);
        if (cartCount) cartCount.textContent = totalItems;

        if (!cartItems) return;

        if (this.items.length === 0) {
            cartItems.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="fa-solid fa-cart-shopping opacity-25 mb-3" style="font-size:2.5rem;"></i>
                    <p class="mb-0">Giỏ hàng trống</p>
                    <small>Thêm sản phẩm để bắt đầu</small>
                </div>`;
        } else {
            cartItems.innerHTML = this.items.map(item => `
                <div class="d-flex align-items-center gap-3 mb-3 pb-3 border-bottom">
                    <!-- Ảnh sản phẩm -->
                    <div style="flex-shrink:0;width:64px;height:64px;border-radius:10px;overflow:hidden;background:#f5f5f5;">
                        <img src="${item.image || NO_IMAGE}" alt="${this.escapeHtml(item.name)}"
                             style="width:100%;height:100%;object-fit:cover;"
                             onerror="this.src='${NO_IMAGE}'">
                    </div>
                    <div class="flex-grow-1 min-w-0">
                        <p class="small fw-bold mb-1 text-truncate">${this.escapeHtml(item.name)}</p>
                        <p class="small text-primary fw-bold mb-2">${this.formatPrice(item.price)}</p>
                        <div class="d-flex align-items-center gap-2">
                            <div class="qty-control d-inline-flex">
                                <button class="qty-btn" onclick="cart.updateQuantity(${item.productId}, ${item.quantity - 1})">−</button>
                                <span class="qty-val">${item.quantity}</span>
                                <button class="qty-btn" onclick="cart.updateQuantity(${item.productId}, ${item.quantity + 1})">+</button>
                            </div>
                        </div>
                    </div>
                    <div class="d-flex flex-column align-items-end gap-1">
                        <span class="small fw-bold text-muted">${this.formatPrice(item.price * item.quantity)}</span>
                        <button class="btn btn-sm btn-outline-danger rounded-circle p-1" onclick="cart.removeItem(${item.productId})"
                                style="width:28px;height:28px;line-height:1;">
                            <i class="fa-solid fa-xmark" style="font-size:0.7rem;"></i>
                        </button>
                    </div>
                </div>
            `).join('');
        }

        this.updateCartTotal();
    }

    updateCartTotal() {
        const totalEl = document.getElementById('cart-total');
        if (!totalEl) return;
        const total = this.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
        totalEl.textContent = this.formatPrice(total);
    }

    formatPrice(price) {
        return new Intl.NumberFormat('vi-VN').format(price) + 'đ';
    }

    escapeHtml(str) {
        return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
    }

    addItem(productId, name, price, image, quantity = 1) {
        const existingItem = this.items.find(item => item.productId === productId);
        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            this.items.push({ productId, name, price: parseFloat(price) || 0, image: image || '', quantity });
        }
        this.saveToStorage();
        this.updateCartUI();
        this.showToast(`✓ Đã thêm "${name}" vào giỏ hàng`, 'success');
        this.openCart();
    }

    removeItem(productId) {
        this.items = this.items.filter(item => item.productId !== productId);
        this.saveToStorage();
        this.updateCartUI();
    }

    updateQuantity(productId, quantity) {
        if (quantity <= 0) {
            this.removeItem(productId);
        } else {
            const item = this.items.find(item => item.productId === productId);
            if (item) item.quantity = quantity;
        }
        this.saveToStorage();
        this.updateCartUI();
    }

    // ========== CHECKOUT (Ajax) ==========
    setupCheckout() {
        const checkoutBtn = document.getElementById('checkout-btn');
        if (!checkoutBtn) return;

        // Hover animation
        checkoutBtn.addEventListener('mouseenter', () => { checkoutBtn.style.transform = 'scale(1.02)'; });
        checkoutBtn.addEventListener('mouseleave', () => { checkoutBtn.style.transform = 'scale(1)'; });

        checkoutBtn.addEventListener('click', () => {
            const receiverName = document.getElementById('cart-receiver-name')?.value?.trim();
            const phone = document.getElementById('cart-phone')?.value?.trim();
            const address = document.getElementById('cart-address')?.value?.trim();

            if (!receiverName || !phone || !address) {
                this.showToast('Vui lòng điền đầy đủ thông tin giao hàng', 'error');
                return;
            }

            if (this.items.length === 0) {
                this.showToast('Giỏ hàng đang trống', 'error');
                return;
            }

            this.checkout(receiverName, phone, address);
        });
    }

    async checkout(receiverName, phone, address) {
        // Kiểm tra nếu chưa đăng nhập
        if (!document.querySelector('meta[name="current-user"]')) {
            this.showToast('Vui lòng đăng nhập để đặt hàng!', 'error');
            setTimeout(() => { window.location.href = '/Account/Login'; }, 1500);
            return;
        }
        const token = document.getElementById('cart-csrf-token')?.value
            || document.querySelector('input[name="__RequestVerificationToken"]')?.value
            || '';

        const requestData = {
            receiverName,
            phone,
            address,
            items: this.items.map(item => ({ productId: item.productId, quantity: item.quantity }))
        };

        const btn = document.getElementById('checkout-btn');
        if (btn) { btn.disabled = true; btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin me-2"></i>Đang xử lý...'; }

        try {
            const response = await fetch(window.orderRoutes?.checkout || '/Order/Checkout', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token || ''
                },
                body: JSON.stringify(requestData)
            });

            const result = await response.json();

            if (result.success) {
                this.showToast(result.message, 'success');
                this.items = [];
                this.saveToStorage();
                this.updateCartUI();
                this.closeCart();
                ['cart-receiver-name','cart-phone','cart-address'].forEach(id => {
                    const el = document.getElementById(id);
                    if (el) el.value = '';
                });

                if (window.orderRoutes?.myOrdersPartial) {
                    this.refreshMyOrders();
                }

                setTimeout(() => window.location.href = '/Order/MyOrders', 1500);
            } else {
                this.showToast(result.message, 'error');
            }
        } catch (error) {
            this.showToast('Lỗi khi xử lý đơn hàng', 'error');
            console.error(error);
        } finally {
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = '<i class="fa-solid fa-bag-shopping me-2"></i>Đặt hàng ngay';
            }
        }
    }

    async refreshMyOrders() {
        const container = document.getElementById('my-orders-container');
        if (!container || !window.orderRoutes?.myOrdersPartial) return;

        try {
            const response = await fetch(window.orderRoutes.myOrdersPartial);
            container.innerHTML = await response.text();
        } catch (error) {
            console.error('Lỗi khi tải lại danh sách đơn hàng:', error);
        }
    }

    // ========== ADD TO CART BUTTONS ==========
    setupProductButtons() {
        document.addEventListener('click', (e) => {
            const btn = e.target.closest('.add-to-cart-btn');
            if (!btn || btn.disabled) return;
            e.preventDefault();

            const productId = parseInt(btn.dataset.productId);
            const name = btn.dataset.productName || `Sản phẩm ${productId}`;
            const price = parseFloat(btn.dataset.productPrice) || 0;
            const image = btn.dataset.productImage || '';

            if (!productId) return;

            // Ripple animation
            btn.classList.add('pulse');
            setTimeout(() => btn.classList.remove('pulse'), 600);

            this.addItem(productId, name, price, image);
        });
    }

    // ========== FILTERS AND SEARCH ==========
    setupFiltersAndSearch() {
        // Support both 'product-search-input' (product page) and 'product-search' (legacy)
        const searchInput = document.getElementById('product-search-input')
            || document.getElementById('product-search');
        if (searchInput) {
            let timer;
            searchInput.addEventListener('input', () => {
                clearTimeout(timer);
                timer = setTimeout(() => this.filterProducts(), 300);
            });
        }

        // Support both data-filter (legacy) and data-type (product page)
        document.querySelectorAll('[data-filter], [data-type]').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('[data-filter], [data-type]').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                this.filterProducts();
            });
        });
    }

    filterProducts() {
        // Support both 'product-search-input' and 'product-search'
        const searchEl = document.getElementById('product-search-input')
            || document.getElementById('product-search');
        const searchVal = (searchEl?.value || '').toLowerCase().trim();

        // Support both data-filter and data-type attributes on active button
        const activeBtn = document.querySelector('[data-filter].active, [data-type].active');
        const activeFilter = activeBtn?.dataset.filter || activeBtn?.dataset.type || 'all';

        document.querySelectorAll('.product-card-wrapper, [data-product-type], .col-md-4, .col-lg-3').forEach(card => {
            // Try multiple ways to get the product name and type
            const name = (card.dataset.productName
                || card.querySelector('h5 a')?.textContent
                || card.querySelector('h5')?.textContent
                || '').toLowerCase();
            const type = (card.dataset.productType
                || card.querySelector('.product-badge')?.textContent
                || '').toLowerCase();

            const matchSearch = !searchVal || name.includes(searchVal);
            const matchFilter = activeFilter === 'all' || type.includes(activeFilter.toLowerCase());

            card.style.display = (matchSearch && matchFilter) ? '' : 'none';
        });
    }

    // ========== FEEDBACK ==========
    setupFeedback() {
        const feedbackForm = document.getElementById('feedback-form');
        if (!feedbackForm) return;

        feedbackForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            // Lấy token từ form hoặc layout
            const token = feedbackForm.querySelector('input[name="__RequestVerificationToken"]')?.value
                || document.querySelector('input[name="__RequestVerificationToken"]')?.value
                || '';

            // Lấy dữ liệu từ form dưới dạng JSON (controller dùng [FromBody])
            const title = feedbackForm.querySelector('[name="Title"]')?.value || '';
            const content = feedbackForm.querySelector('[name="Content"]')?.value || '';

            const submitUrl = (window.feedbackRoutes && window.feedbackRoutes.submit)
                ? window.feedbackRoutes.submit
                : '/Feedback/Submit';

            try {
                const response = await fetch(submitUrl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token
                    },
                    body: JSON.stringify({ title, content })
                });

                const result = await response.json();
                this.showToast(result.message, result.success ? 'success' : 'error');
                if (result.success) {
                    feedbackForm.reset();
                    // Refresh lịch sử phản hồi nếu có
                    if (window.feedbackRoutes && window.feedbackRoutes.myFeedbackPartial) {
                        try {
                            const r = await fetch(window.feedbackRoutes.myFeedbackPartial);
                            const container = document.getElementById('feedback-history-container');
                            if (container) container.innerHTML = await r.text();
                        } catch (err) { console.error('Lỗi refresh feedback:', err); }
                    }
                }
            } catch (error) {
                this.showToast('Lỗi khi gửi phản hồi', 'error');
                console.error(error);
            }
        });
    }

    // ========== TOAST ==========
    showToast(message, type = 'info') {
        let container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.style.cssText = 'position:fixed;top:20px;right:20px;z-index:9999;display:flex;flex-direction:column;gap:8px;';
            document.body.appendChild(container);
        }

        const colors = { success: '#16a34a', error: '#dc2626', info: '#2563eb', warning: '#d97706' };
        const toast = document.createElement('div');
        toast.style.cssText = `
            padding: 0.75rem 1.25rem;
            border-radius: 12px;
            background: ${colors[type] || colors.info};
            color: white;
            font-weight: 600;
            font-size: 0.875rem;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
            animation: slideInRight 0.3s ease;
            max-width: 320px;
        `;
        toast.textContent = message;
        container.appendChild(toast);

        setTimeout(() => {
            toast.style.animation = 'fadeOut 0.3s ease';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
}

// ===== USER CHAT SYSTEM (localStorage) =====
class UserChat {
    constructor() {
        this.currentPartner = null;
        this.currentUsername = null;
        this.isOpen = false;
        this.init();
    }

    init() {
        // Lấy username hiện tại từ meta tag nếu có
        const userMeta = document.querySelector('meta[name="current-user"]');
        this.currentUsername = userMeta?.content || null;
        if (!this.currentUsername) return;

        this.createChatWidget();
        this.checkUnread();
    }

    createChatWidget() {
        const widget = document.createElement('div');
        widget.id = 'user-chat-widget';
        widget.innerHTML = `
            <style>
            #user-chat-widget { position: fixed; bottom: 20px; right: 20px; z-index: 1500; font-family: 'Inter', sans-serif; }
            #chat-fab { width: 54px; height: 54px; border-radius: 50%; background: var(--primary-color,#8b5e3c);
                border: none; cursor: pointer; box-shadow: 0 4px 20px rgba(0,0,0,0.3);
                display: flex; align-items: center; justify-content: center; color: white; font-size: 1.2rem;
                transition: transform 0.2s; position: relative; }
            #chat-fab:hover { transform: scale(1.1); }
            #chat-unread-dot { position: absolute; top: 4px; right: 4px; width: 14px; height: 14px;
                background: #ef4444; border-radius: 50%; border: 2px solid white; display: none;
                font-size: 0.5rem; font-weight: 700; color: white; align-items: center; justify-content: center; }
            #user-chat-panel { position: absolute; bottom: 64px; right: 0; width: 340px; height: 460px;
                background: white; border-radius: 16px; box-shadow: 0 8px 40px rgba(0,0,0,0.2);
                display: none; flex-direction: column; overflow: hidden; }
            #user-chat-panel.open { display: flex; }
            .uchat-header { background: var(--primary-color,#8b5e3c); color: white; padding: 0.875rem 1rem;
                display: flex; align-items: center; gap: 0.5rem; }
            .uchat-header h6 { margin: 0; font-weight: 700; font-size: 0.9rem; }
            .uchat-tabs { display: flex; border-bottom: 1px solid #f0f0f0; }
            .uchat-tab { flex: 1; padding: 0.5rem; text-align: center; font-size: 0.8rem; font-weight: 600;
                cursor: pointer; color: #9ca3af; border-bottom: 2px solid transparent; transition: 0.2s; }
            .uchat-tab.active { color: var(--primary-color,#8b5e3c); border-bottom-color: var(--primary-color,#8b5e3c); }
            .uchat-content { flex: 1; overflow: hidden; display: flex; flex-direction: column; }
            .uchat-user-list { flex: 1; overflow-y: auto; }
            .uchat-user-item { display: flex; align-items: center; gap: 0.65rem; padding: 0.75rem 1rem;
                cursor: pointer; border-bottom: 1px solid #f8f9fa; transition: background 0.15s; }
            .uchat-user-item:hover { background: #fdf8f5; }
            .uchat-user-item.active { background: #fff3e0; }
            .uchat-avatar { width: 38px; height: 38px; border-radius: 50%; object-fit: cover;
                background: #8b5e3c; display: flex; align-items: center; justify-content: center;
                color: white; font-weight: 700; font-size: 0.9rem; flex-shrink: 0; }
            .uchat-messages { flex: 1; overflow-y: auto; padding: 0.75rem; display: flex; flex-direction: column; gap: 0.4rem; }
            .uchat-msg { padding: 0.4rem 0.7rem; border-radius: 12px; font-size: 0.82rem;
                max-width: 80%; word-break: break-word; }
            .uchat-msg.mine { background: var(--primary-color,#8b5e3c); color: white;
                align-self: flex-end; border-radius: 12px 12px 0 12px; }
            .uchat-msg.theirs { background: #f3f4f6; color: #1f2937;
                align-self: flex-start; border-radius: 12px 12px 12px 0; }
            .uchat-msg-time { font-size: 0.6rem; opacity: 0.7; margin-top: 2px; }
            .uchat-input-row { display: flex; gap: 0.4rem; padding: 0.6rem; border-top: 1px solid #f0f0f0; }
            .uchat-input { flex: 1; border: 1px solid #e5e7eb; border-radius: 20px; padding: 0.4rem 0.85rem;
                font-size: 0.83rem; outline: none; }
            .uchat-send { width: 34px; height: 34px; border-radius: 50%; background: var(--primary-color,#8b5e3c);
                border: none; cursor: pointer; color: white; display: flex; align-items: center; justify-content: center; }
            </style>
            <button id="chat-fab" onclick="userChat.togglePanel()" title="Chat">
                <i class="fa-solid fa-comment-dots"></i>
                <span id="chat-unread-dot"></span>
            </button>
            <div id="user-chat-panel">
                <div class="uchat-header">
                    <i class="fa-solid fa-comments"></i>
                    <h6 id="uchat-title">Tin nhắn</h6>
                    <button onclick="userChat.togglePanel()" style="margin-left:auto;background:transparent;border:none;color:white;font-size:1rem;cursor:pointer;">×</button>
                </div>
                <div class="uchat-tabs">
                    <div class="uchat-tab active" id="tab-contacts" onclick="userChat.showContacts()">
                        <i class="fa-solid fa-users me-1"></i>Liên hệ
                    </div>
                    <div class="uchat-tab" id="tab-admin" onclick="userChat.openAdminChat()">
                        <i class="fa-solid fa-headset me-1"></i>Admin
                    </div>
                </div>
                <div class="uchat-content" id="uchat-content">
                    <div class="uchat-user-list" id="uchat-contacts">
                        <div style="padding:1rem;text-align:center;color:#9ca3af;font-size:0.8rem;">
                            <i class="fa-solid fa-users mb-2" style="font-size:1.5rem;opacity:0.3;"></i>
                            <br>Chưa có cuộc hội thoại nào
                        </div>
                    </div>
                    <div id="uchat-chat-view" style="display:none;flex:1;flex-direction:column;overflow:hidden;">
                        <div style="display:flex;align-items:center;gap:0.5rem;padding:0.5rem 0.75rem;border-bottom:1px solid #f0f0f0;">
                            <button onclick="userChat.showContacts()" style="background:transparent;border:none;color:#9ca3af;cursor:pointer;padding:0;">
                                <i class="fa-solid fa-arrow-left"></i>
                            </button>
                            <span id="uchat-partner-name" style="font-weight:600;font-size:0.85rem;"></span>
                        </div>
                        <div class="uchat-messages" id="uchat-messages"></div>
                        <div class="uchat-input-row">
                            <input class="uchat-input" id="uchat-input" placeholder="Nhập tin nhắn..."
                                   onkeydown="if(event.key==='Enter') userChat.sendMessage()">
                            <button class="uchat-send" onclick="userChat.sendMessage()">
                                <i class="fa-solid fa-paper-plane" style="font-size:0.75rem;"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        document.body.appendChild(widget);

        // Load contacts
        setTimeout(() => this.loadContacts(), 500);
    }

    togglePanel() {
        this.isOpen = !this.isOpen;
        const panel = document.getElementById('user-chat-panel');
        panel.classList.toggle('open', this.isOpen);
        if (this.isOpen) this.loadContacts();
    }

    loadContacts() {
        // Thu thập các cuộc hội thoại từ localStorage
        const contactsEl = document.getElementById('uchat-contacts');
        if (!contactsEl) return;

        const convos = [];
        try {
            for (let i = 0; i < localStorage.length; i++) {
                const k = localStorage.key(i);
                if (k && k.startsWith('userchat_' + this.currentUsername + '_')) {
                    const partner = k.replace('userchat_' + this.currentUsername + '_', '');
                    try {
                        const msgs = JSON.parse(localStorage.getItem(k) || '[]') || [];
                        const unread = msgs.filter(m => m.from !== this.currentUsername && !m.read).length;
                        convos.push({ partner, unread, lastMsg: msgs[msgs.length-1] });
                    } catch(e){}
                }
            }
        } catch(e) {}

        if (convos.length === 0) {
            contactsEl.innerHTML = `<div style="padding:1.5rem;text-align:center;color:#9ca3af;font-size:0.8rem;">
                <i class="fa-solid fa-comment-dots mb-2" style="font-size:1.5rem;opacity:0.3;display:block;"></i>
                Chưa có cuộc hội thoại nào<br>
                <small>Chat với Admin để bắt đầu</small>
            </div>`;
            return;
        }

        contactsEl.innerHTML = convos.map(c => `
            <div class="uchat-user-item ${this.currentPartner === c.partner ? 'active' : ''}"
                 onclick="userChat.openChat('${c.partner}')">
                <div class="uchat-avatar">${c.partner[0].toUpperCase()}</div>
                <div style="flex:1;min-width:0;">
                    <div style="font-weight:600;font-size:0.82rem;">${c.partner}</div>
                    ${c.lastMsg ? `<div style="font-size:0.72rem;color:#9ca3af;overflow:hidden;white-space:nowrap;text-overflow:ellipsis;">${c.lastMsg.text}</div>` : ''}
                </div>
                ${c.unread > 0 ? `<span style="background:#ef4444;color:white;border-radius:10px;font-size:0.65rem;padding:1px 6px;font-weight:700;">${c.unread}</span>` : ''}
            </div>
        `).join('');
    }

    openChat(partner) {
        this.currentPartner = partner;
        document.getElementById('uchat-contacts').style.display = 'none';
        const chatView = document.getElementById('uchat-chat-view');
        chatView.style.display = 'flex';
        document.getElementById('uchat-partner-name').textContent = partner;
        document.getElementById('tab-contacts').classList.remove('active');
        this.renderMessages();
    }

    openAdminChat() {
        this.openChat('Admin');
        document.getElementById('tab-admin').classList.add('active');
        document.getElementById('tab-contacts').classList.remove('active');
    }

    showContacts() {
        this.currentPartner = null;
        document.getElementById('uchat-contacts').style.display = '';
        document.getElementById('uchat-chat-view').style.display = 'none';
        document.getElementById('uchat-partner-name').textContent = '';
        document.getElementById('tab-contacts').classList.add('active');
        document.getElementById('tab-admin').classList.remove('active');
        this.loadContacts();
    }

    getKey() {
        return `userchat_${this.currentUsername}_${this.currentPartner}`;
    }

    renderMessages() {
        const container = document.getElementById('uchat-messages');
        if (!container || !this.currentPartner) return;

        const key = this.getKey();
        const msgs = JSON.parse(localStorage.getItem(key) || '[]');

        // Nếu chat với Admin, cũng lấy tin nhắn từ admin chat storage
        let adminMsgs = [];
        if (this.currentPartner === 'Admin') {
            const adminKey = 'chat_' + this.currentUsername;
            try { adminMsgs = JSON.parse(localStorage.getItem(adminKey) || '[]'); } catch(e){}
        }

        const allMsgs = this.currentPartner === 'Admin'
            ? adminMsgs.map(m => ({ from: m.fromUser ? this.currentUsername : 'Admin', text: m.text, time: m.time }))
            : msgs;

        if (!allMsgs.length) {
            container.innerHTML = `<div style="text-align:center;color:#9ca3af;font-size:0.8rem;margin-top:2rem;">Bắt đầu cuộc trò chuyện với <b>${this.currentPartner}</b></div>`;
        } else {
            container.innerHTML = allMsgs.map(m => `
                <div>
                    <div style="font-size:0.6rem;color:#9ca3af;text-align:${m.from===this.currentUsername?'right':'left'};margin-bottom:2px;">${m.time}</div>
                    <div class="uchat-msg ${m.from===this.currentUsername?'mine':'theirs'}">${this.escapeHtml(m.text)}</div>
                </div>
            `).join('');
        }
        container.scrollTop = container.scrollHeight;

        // Mark as read
        if (msgs.length && this.currentPartner !== 'Admin') {
            try {
                const updated = msgs.map(m => ({ ...m, read: true }));
                localStorage.setItem(key, JSON.stringify(updated));
            } catch(e) {}
        }
    }

    sendMessage() {
        const input = document.getElementById('uchat-input');
        if (!input || !input.value.trim() || !this.currentPartner) return;

        const time = new Date().toLocaleTimeString('vi-VN', {hour:'2-digit',minute:'2-digit'});

        try {
            if (this.currentPartner === 'Admin') {
                const adminKey = 'chat_' + this.currentUsername;
                const msgs = JSON.parse(localStorage.getItem(adminKey) || '[]');
                msgs.push({ text: input.value.trim(), fromUser: true, time, readByAdmin: false });
                localStorage.setItem(adminKey, JSON.stringify(msgs));
            } else {
                const key = this.getKey();
                const msgs = JSON.parse(localStorage.getItem(key) || '[]');
                msgs.push({ from: this.currentUsername, text: input.value.trim(), time, read: false });
                localStorage.setItem(key, JSON.stringify(msgs));
            }
        } catch(e) { console.warn('localStorage không khả dụng:', e); }

        input.value = '';
        this.renderMessages();
    }

    checkUnread() {
        let unread = 0;
        for (let k in localStorage) {
            if (k.startsWith('chat_') || k.startsWith('userchat_')) {
                try {
                    const msgs = JSON.parse(localStorage.getItem(k) || '[]') || [];
                    unread += msgs.filter(m => (m.from !== this.currentUsername) && !m.read && !m.readByUser).length;
                } catch(e){}
            }
        }
        const dot = document.getElementById('chat-unread-dot');
        if (dot) {
            dot.style.display = unread > 0 ? 'flex' : 'none';
        }
    }

    escapeHtml(str) {
        return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
    }
}

// ===== TOAST CSS =====
const toastStyle = document.createElement('style');
toastStyle.textContent = `
@keyframes slideInRight {
    from { transform: translateX(100px); opacity: 0; }
    to   { transform: translateX(0);    opacity: 1; }
}
@keyframes fadeOut {
    from { opacity: 1; }
    to   { opacity: 0; transform: translateY(-10px); }
}
.qty-control { display: inline-flex; align-items: center; border: 1px solid #e0d5c9; border-radius: 20px; overflow: hidden; }
.qty-btn { background: transparent; border: none; padding: 2px 8px; cursor: pointer; font-size: 1rem; font-weight: 700; color: #8b5e3c; transition: background 0.15s; }
.qty-btn:hover { background: #fff3e0; }
.qty-val { padding: 2px 8px; font-weight: 700; font-size: 0.9rem; }
.pulse { animation: pulse-anim 0.4s ease; }
@keyframes pulse-anim { 0%,100%{transform:scale(1)} 50%{transform:scale(1.15)} }
`;
document.head.appendChild(toastStyle);

// ===== INIT =====
const cart = new CartManager();
let userChat;
document.addEventListener('DOMContentLoaded', () => {
    // Chỉ init user chat khi có user đăng nhập
    if (document.querySelector('meta[name="current-user"]')) {
        userChat = new UserChat();
    }
});
