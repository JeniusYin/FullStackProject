import axiosRequest from "../plugins/axios"

const api = {
    getTest(params){
        return axiosRequest({
            url: "v1.0/test/test",
            method: "get",
            params
        })
    }
}

export default api