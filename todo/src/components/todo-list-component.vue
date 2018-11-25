<template>
  <div>
    <b-input-group prepend="Заметка">
      <b-form-input v-model="inputValue" @keydown.native="onKeyDown" />
      <b-input-group-append>
        <b-btn variant="info" :disabled="emptyInput" @click="onAddClick">Добавить</b-btn>
      </b-input-group-append>
    </b-input-group>
    <br/>
    <b-table striped hover :items="items" :fields="fields">
      <template slot="actions" slot-scope="row">
        <b-button size="sm" @click.stop="deleteItem(row.item)" class="mr-1">
          Удалить
        </b-button>
      </template>
    </b-table>
  </div>
</template>

<script>
export default {
  name: 'TodoListComponent',
  props: {
    items: {
      type: Array,
      required: true
    }
  },
  data () {
    return {
      inputValue: null,
      fields: {
        'label': {
          label: 'Текст',
          sortable: true
        },
        'timestamp': {
          label: 'Время',
          sortable: false,
          formatter: value => {
            let h = this.zeroPad(value.getHours(), 2);
            let m = this.zeroPad(value.getMinutes(), 2);
            let s = this.zeroPad(value.getSeconds(), 2);

            return `${h}:${m}:${s}`;
          }
        },
        'actions': {
          label: '',
          sortable: false
        }
      }
    }
  },
  computed: {
    emptyInput() {
      return this.inputValue === null
        || this.inputValue === undefined
        || this.inputValue.trim().length == 0;
    }
  },
  methods: {
    addItem() {
      if (this.emptyInput)
        return;

      this.items.push({
        label: this.inputValue,
        timestamp: new Date()
      });
      this.$emit('changed');

      let vm = this;

      setTimeout(() => vm.inputValue = null, 0);
    },

    deleteItem(item) {
      let index = this.items.indexOf(item);

      if (index !== -1)
        this.items.splice(index, 1);
    },

    zeroPad(number, width) {
      width -= number.toString().length;

      if (width > 0)
        return new Array( width + (/\./.test( number ) ? 2 : 1) ).join( '0' ) + number;
      
      return number + "";
    },

    onKeyDown(e) {
      if (e.which === 13)
        this.addItem();
    },

    onAddClick(e) {
      this.addItem();
    }
  }
}
</script>
