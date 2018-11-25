<template>
  <div id="app-container">
    <div>
      <h1>Всего заметок: {{todoList.length}} <sup v-if="dirtyFlag">*</sup></h1>
      <todo-list-component :items="todoList" @changed="onListChanged" />
    </div>
  </div>
</template>

<script>
import TodoListComponent from './components/todo-list-component';

export default {
  name: 'App',
  components: {
    TodoListComponent
  },
  data () {
    return {
      todoList: [],
      dirtyFlag: false
    }
  },
  methods: {
    onListChanged() {
      this.dirtyFlag = true;
    }
  }
}
</script>

<style>
#app-container {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}
</style>
