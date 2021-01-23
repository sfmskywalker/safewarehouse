const db = idb.openDB('SafeWarehouse', 1, {
    upgrade(db) {
        db.createObjectStore('materials', {keyPath: 'id'}).createIndex('id', 'id', {unique: true});
        db.createObjectStore('damageTypes', {keyPath: 'id'}).createIndex('id', 'id', {unique: true});
        db.createObjectStore('reports', {keyPath: 'id'}).createIndex('id', 'id', {unique: true});
    },
});

function Store(name) {
    this.name = name;
    this.get = async (key) => (await db).transaction(this.name).store.get(key);
    this.getAll = async () => (await db).transaction(this.name).store.getAll();
    this.put = async (value) => (await db).transaction(this.name, 'readwrite').store.put(value);
    this.delete = async (key) => (await db).transaction(this.name, 'readwrite').store.delete(key);
}

export const materials = new Store('materials');
export const damageTypes = new Store('damageTypes');
export const reports = new Store('reports');