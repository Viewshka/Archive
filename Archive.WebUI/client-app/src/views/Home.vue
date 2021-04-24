<template>
  <div class="home">
    <h2>Главная страница</h2>
    <h3>Новости:</h3>
    <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit. Beatae blanditiis impedit ipsa molestias, nesciunt non odio officiis quod ratione similique! Accusamus magnam numquam odio quae? Ad adipisci, assumenda autem corporis culpa doloremque dolores ducimus expedita fugit harum illo incidunt ipsa ipsum iste maiores modi molestias necessitatibus nesciunt omnis optio pariatur praesentium provident quasi quod quos ratione, repudiandae saepe similique sunt suscipit tempora veniam voluptates voluptatibus! Accusamus accusantium aperiam cupiditate dicta dignissimos, dolorem dolores eaque eius facere, fuga fugiat illum ipsa iste labore mollitia nemo neque nihil nisi non odit pariatur porro possimus provident quis repellendus sapiente soluta totam veritatis voluptatum.</p>
  </div>
</template>

<script>

export default {
  name: 'Home',
  data() {
    return {}
  },
  components: {}
}
</script>
<style lang="scss">
.home {
  height: calc(100vh - 150px);
  margin-left: 10px;
}

.demo-container, #page {
  height: 100%;
  min-height: 300px;
}

.item {
  height: 100%;
}

#page p {
  font-size: 20px;
  padding-top: 10px;
  text-align: center;
}
</style>

