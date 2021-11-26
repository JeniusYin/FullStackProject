"use strict";

import axios from "axios";
import router from "../router/index"
import { getToken } from "../plugins/cookie"
import { Message, Loading } from "element-ui"

// Full config:  https://github.com/axios/axios#request-config
// axios.defaults.baseURL = process.env.baseURL || process.env.apiUrl || '';
// axios.defaults.headers.common['Authorization'] = AUTH_TOKEN;
// axios.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded';

let loadingInstance = "";

let config = {
  baseURL: process.env.VUE_APP_BaseURL,
  timeout: 60 * 1000, // Timeout
  //withCredentials: true, // Check cross-site Access-Control
  headers:{ "Content-Type" : "application/json"}
};

const axiosRequest = axios.create(config);

axiosRequest.interceptors.request.use(
  function(config) {
    loadingInstance = Loading.service({
      text: "正在加载中~",
      background: "rgba(255,255,225,0.3)"
    })

    if (getToken()) {
      const token_admin = JSON.parse(getToken()).token;
      config.headers.common["Authorization"] = `bearer ${token_admin}`;
    }

    if (config.method === "get") {
      config.data = true;
    }
    return config;
  },
  function(error) {
    // Do something with request error
    return Promise.reject(error);
  }
);

// Add a response interceptor
axiosRequest.interceptors.response.use(
  function(response) {
    loadingInstance.close();
    if (response.status === 401) {
      Message.error({
        showClose: true,
        message: "请重新登录！"
      });
      router.push("/login");
    }else if (response.status === 403) {
      Message.error({
        showClose: true,
        message: "角色错误，请重新登录！"
      });
      router.push("/login");
    }else if (response.status === 200) {
      return response.data;
    }
    return response;
  },
  function(error) {
    loadingInstance.close()
    Message.error({
        showClose: true,
        message: "发生错误，请重试"
    })
    return Promise.reject(error);
  }
);

export default axiosRequest;
