// 微信小游戏API Mock适配器
// 用于在非微信环境中运行时提供兼容支持

(function() {
    // 检查是否已经存在wx对象（真实的微信环境）
    if (typeof wx !== 'undefined' && wx.getSystemInfoSync) {
        console.log('[WX-Mock] 检测到真实微信环境，不需要Mock');
        return;
    }

    console.log('[WX-Mock] 初始化微信API Mock适配器');

    // 创建wx对象
    window.wx = window.wx || {};

    // Mock getNetworkType
    if (!wx.getNetworkType) {
        wx.getNetworkType = function(options) {
            console.warn('[WX-Mock] getNetworkType called - returning mock data');
            const result = {
                networkType: 'wifi',
                errMsg: 'getNetworkType:ok'
            };
            if (options && typeof options.success === 'function') {
                setTimeout(() => options.success(result), 0);
            }
            if (options && typeof options.complete === 'function') {
                setTimeout(() => options.complete(result), 0);
            }
        };
    }

    // Mock getPermissionBytes
    if (!wx.getPermissionBytes) {
        wx.getPermissionBytes = function(options) {
            console.warn('[WX-Mock] getPermissionBytes called - returning mock data');
            const result = {
                permissionBytes: '',
                errMsg: 'getPermissionBytes:ok'
            };
            if (options && typeof options.success === 'function') {
                setTimeout(() => options.success(result), 0);
            }
            if (options && typeof options.complete === 'function') {
                setTimeout(() => options.complete(result), 0);
            }
        };
    }

    // Mock getABTestConfig
    if (!wx.getABTestConfig) {
        wx.getABTestConfig = function(options) {
            console.warn('[WX-Mock] getABTestConfig called - returning mock data');
            const result = {
                config: {},
                errMsg: 'getABTestConfig:ok'
            };
            if (options && typeof options.success === 'function') {
                setTimeout(() => options.success(result), 0);
            }
            if (options && typeof options.complete === 'function') {
                setTimeout(() => options.complete(result), 0);
            }
        };
    }

    // Mock getSystemInfoSync
    if (!wx.getSystemInfoSync) {
        wx.getSystemInfoSync = function() {
            console.warn('[WX-Mock] getSystemInfoSync called - returning mock data');
            return {
                brand: 'mock',
                model: 'mock',
                pixelRatio: window.devicePixelRatio || 1,
                screenWidth: window.screen.width,
                screenHeight: window.screen.height,
                windowWidth: window.innerWidth,
                windowHeight: window.innerHeight,
                statusBarHeight: 0,
                language: navigator.language || 'zh_CN',
                version: '8.0.0',
                system: 'Mock 1.0.0',
                platform: 'devtools',
                fontSizeSetting: 16,
                SDKVersion: '2.0.0',
                benchmarkLevel: 1,
                albumAuthorized: true,
                cameraAuthorized: true,
                locationAuthorized: true,
                microphoneAuthorized: true,
                notificationAuthorized: true,
                notificationAlertAuthorized: true,
                notificationBadgeAuthorized: true,
                notificationSoundAuthorized: true,
                phoneCalendarAuthorized: true
            };
        };
    }

    // Mock getLaunchOptionsSync
    if (!wx.getLaunchOptionsSync) {
        wx.getLaunchOptionsSync = function() {
            console.warn('[WX-Mock] getLaunchOptionsSync called - returning mock data');
            return {
                scene: 1001,
                query: {},
                shareTicket: '',
                referrerInfo: {}
            };
        };
    }

    // Mock onShow
    if (!wx.onShow) {
        wx.onShow = function(callback) {
            console.warn('[WX-Mock] onShow registered');
            // 在浏览器环境中，可以监听页面可见性变化
            document.addEventListener('visibilitychange', function() {
                if (document.visibilityState === 'visible' && typeof callback === 'function') {
                    callback({});
                }
            });
        };
    }

    // Mock onHide
    if (!wx.onHide) {
        wx.onHide = function(callback) {
            console.warn('[WX-Mock] onHide registered');
            document.addEventListener('visibilitychange', function() {
                if (document.visibilityState === 'hidden' && typeof callback === 'function') {
                    callback({});
                }
            });
        };
    }

    // Mock request
    if (!wx.request) {
        wx.request = function(options) {
            console.warn('[WX-Mock] request called:', options.url);
            fetch(options.url, {
                method: options.method || 'GET',
                headers: options.header || {},
                body: options.data ? JSON.stringify(options.data) : undefined
            })
            .then(response => response.json())
            .then(data => {
                if (options.success) {
                    options.success({ data: data, statusCode: 200, header: {} });
                }
                if (options.complete) {
                    options.complete({ data: data, statusCode: 200, header: {} });
                }
            })
            .catch(error => {
                if (options.fail) {
                    options.fail({ errMsg: error.message });
                }
                if (options.complete) {
                    options.complete({ errMsg: error.message });
                }
            });
        };
    }

    console.log('[WX-Mock] 微信API Mock适配器初始化完成');
})();
