<template>
  <div class="yahaha-form">
      <div class="head">
          <span class="title">{{ title }}</span>
          <el-button-group>
              <el-button type="primary" @click="openAdd" v-auth="'inventory:add'"> 新增 </el-button>
              <el-button type="primary" @click="deleteRow()" v-auth="'inventory:add'"> 删除 </el-button>
          </el-button-group>
      </div>
      <div class="body">
          <form :style="{ width: formWidth, height: formHeight }">
              <label for="name">Name:</label>
              <input v-model="name" id="name" type="text" />

              <label for="email">Email:</label>
              <input v-model="email" id="email" type="email" />


              <button @click.prevent="submitForm">Submit</button>
          </form>
      </div>
  </div>
  <!-- <el-card>
    <form >
        <label for="name">Name:</label>
        <input v-model="name" id="name" type="text" />

        <label for="email">Email:</label>
        <input v-model="email" id="email" type="email" />


        <button @click.prevent="submitForm">Submit</button>
      </form>
  </el-card> -->
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';

const parentWidth = ref<number>(0);
const parentHeight = ref<number>(0);
const formWidth = ref<string>('80%');
const formHeight = ref<string>('100%');
const title = ref<string>('新建');
const name = ref<string>('');
const email = ref<string>('');

const handleResize = () => {
  parentWidth.value = window.innerWidth;
  parentHeight.value = window.innerHeight;
};



onMounted(() => {
  handleResize();
  window.addEventListener('resize', handleResize);
});

const submitForm = () => {
  console.log('Form submitted:', { name: name.value, email: email.value });
};
</script>

<style scoped>
.yahaha-form {
  background-color: #fff;
  display: flex;
  flex-direction: column;
  height: 100%;
  width: 100%;
  border-radius: 5px;
  transition: all 0.9s;
  border: 1px solid var(--el-border-color);
  
  .head {
      display: flex;
      justify-content: space-between;
      padding: 8px;
      background-color: rgb(245, 245, 245);
      border-top-left-radius: 4px;
      border-top-right-radius: 4px;
  }

  .title {
      color: var(--el-color-primary);
      font-weight: bold;
      font-size: 18px;
      /* margin-bottom: 10px; */
  }
}

.yahaha-form:hover {
  box-shadow: var(--el-box-shadow-light);
}

.body {
  padding: 8px;
}
</style>
