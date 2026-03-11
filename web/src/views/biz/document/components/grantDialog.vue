<template>
  <div>
    <form-container v-model="visible" title="目录授权" form-size="1100px">
      <el-tabs v-model="activeTab">
        <el-tab-pane label="用户" name="users">
          <div class="toolbar">
            <el-button type="primary" @click="openUserSelector">选择用户</el-button>
            <el-button @click="saveUsers">保存用户授权</el-button>
          </div>
          <el-table :data="users" border>
            <el-table-column prop="account" label="账号" min-width="140" />
            <el-table-column prop="name" label="姓名" min-width="120" />
            <el-table-column label="操作" width="100" align="center">
              <template #default="scope">
                <el-button link type="danger" @click="removeUser(scope.row.id)">移除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
        <el-tab-pane label="角色" name="roles">
          <div class="toolbar">
            <el-button type="primary" @click="openRoleSelector">选择角色</el-button>
            <el-button @click="saveRoles">保存角色授权</el-button>
          </div>
          <el-table :data="roles" border>
            <el-table-column prop="name" label="角色名称" min-width="160" />
            <el-table-column prop="code" label="角色编码" min-width="140" />
            <el-table-column label="操作" width="100" align="center">
              <template #default="scope">
                <el-button link type="danger" @click="removeRole(scope.row.id)">移除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <el-button @click="visible = false">关闭</el-button>
      </template>
    </form-container>

    <user-selector
      ref="userSelectorRef"
      biz
      multiple
      :org-tree-api="bizOrgApi.tree"
      :position-tree-api="bizPositionApi.tree"
      :role-tree-api="bizRoleApi.tree"
      :user-selector-api="bizUserApi.selector"
      @successful="handleChooseUser"
    />
    <role-selector
      ref="roleSelectorRef"
      multiple
      :org-tree-api="bizOrgApi.tree"
      :role-selector-api="bizRoleApi.roleSelector"
      @successful="handleChooseRole"
    />
  </div>
</template>

<script setup lang="ts">
import { SysRole, SysUser, bizOrgApi, bizPositionApi, bizRoleApi, bizUserApi, documentApi } from "@/api";
import { UserSelectorInstance } from "@/components/Selectors/UserSelector/interface";
import { RoleSelectorInstance } from "@/components/Selectors/RoleSelector/interface";

interface OpenPayload {
  id: number;
}

const visible = ref(false);
const activeTab = ref("users");
const currentId = ref(0);
const users = ref<SysUser.SysUserInfo[]>([]);
const roles = ref<SysRole.SysRoleInfo[]>([]);
const userSelectorRef = ref<UserSelectorInstance>();
const roleSelectorRef = ref<RoleSelectorInstance>();

async function onOpen(payload: OpenPayload) {
  currentId.value = payload.id;
  activeTab.value = "users";
  const { data } = await documentApi.grantDetail({ id: payload.id });
  users.value = data.users || [];
  roles.value = data.roles || [];
  visible.value = true;
}

function openUserSelector() {
  userSelectorRef.value?.showSelector(users.value);
}

function openRoleSelector() {
  roleSelectorRef.value?.showSelector(roles.value);
}

function handleChooseUser(data: SysUser.SysUserInfo[]) {
  users.value = data;
}

function handleChooseRole(data: SysRole.SysRoleInfo[]) {
  roles.value = data;
}

function removeUser(id: number | string) {
  users.value = users.value.filter(item => item.id !== id);
}

function removeRole(id: number | string) {
  roles.value = roles.value.filter(item => item.id !== id);
}

async function saveUsers() {
  await documentApi.grantUsers({ id: currentId.value, userIds: users.value.map(item => Number(item.id)) });
}

async function saveRoles() {
  await documentApi.grantRoles({ id: currentId.value, roleIds: roles.value.map(item => Number(item.id)) });
}

defineExpose({ onOpen });
</script>

<style scoped lang="scss">
.toolbar {
  margin-bottom: 12px;
}
</style>
