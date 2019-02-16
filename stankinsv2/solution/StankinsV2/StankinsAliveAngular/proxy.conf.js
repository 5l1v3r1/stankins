const PROXY_CONFIG = [
    {
      context: [
          "/DataHub" , 
          "/api",                
      ],
      target: "http://localhost:5000/",
      secure: false,
      "changeOrigin": true,
      "logLevel": "debug",
      "ws":true
    }
]
module.exports = PROXY_CONFIG;
