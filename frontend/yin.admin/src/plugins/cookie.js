import Cookie from "js-cookie"

export function getToken(){
    return Cookie.get("token_admin")
}

export function setToken(token){
    return Cookie.set("token_admin", token)
}

export function removeToken(){
    return Cookie.remove("token_admin")
}