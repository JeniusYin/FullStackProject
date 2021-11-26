<template>
  <div class="about">
    <h1>This is an about page</h1>
    <h2>{{ data }}</h2>
  </div>
</template>
<script>

export default ({
  data() {
    return {
      message: "",
      data: ""
    }
  },
  methods:{
    getTest(){
      this.$api.getTest()
      .then(res => {
        this.message = res.message,
        this.data = res.data
      })
    }
  },
  created(){
    this.getTest()
  }
})
</script>

