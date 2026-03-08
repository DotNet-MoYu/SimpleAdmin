<template>
  <div class="home card">
    <!-- 如需恢复原欢迎图首页，取消下一行注释，并可按需注释广告面板 -->
    <!-- <img class="home-bg" src="@/assets/images/welcome.png" alt="welcome" /> -->
    <div class="ad-section">
      <a
        v-for="(item, index) in adCards"
        :key="item.title"
        :class="['ad-card', `ad-card--${index + 1}`]"
        :href="item.link"
        target="_blank"
        rel="noopener noreferrer"
      >
        <div class="ad-meta">
          <span class="ad-tag">{{ item.tag }}</span>
          <span class="ad-note">{{ item.note }}</span>
        </div>
        <div class="ad-title-row">
          <span class="ad-icon">{{ item.icon }}</span>
          <span class="ad-title">{{ item.title }}</span>
        </div>
        <div class="ad-desc">{{ item.desc }}</div>
        <div class="ad-price">{{ item.price }}</div>
        <ul class="ad-points">
          <li v-for="point in item.points" :key="point">{{ point }}</li>
        </ul>
        <div class="ad-link">
          <span>{{ item.cta }}</span>
          <span class="ad-arrow">→</span>
        </div>
      </a>
    </div>
  </div>
</template>

<script setup lang="ts" name="home">
const adCards = [
  {
    icon: "A",
    tag: "网络加速",
    note: "邀请推荐",
    title: "性价比机场推荐",
    desc: "面向日常访问和开发场景的稳定线路。",
    price: "12 元 / 月 · 128G",
    points: ["月付门槛低，适合轻量到中度使用。", "纯净IP，畅用AI。", "覆盖常见访问场景，上手成本低。"],
    cta: "查看方案",
    link: "https://www.zou666.net/#/register?code=hfkhGLG5"
  },
  {
    icon: "AI",
    tag: "AI 工具",
    note: "邀请推荐",
    title: "性价比AI中转站推荐",
    desc: "无需梯子，快速接入常用模型调用能力。",
    price: "支持 Codex 中转 · 价格低到爆",
    points: ["无需梯子即可访问，接入门槛更低。", "低门槛接入，开箱即用。", "价格低到爆，适合长期使用。"],
    cta: "查看方案",
    link: "https://teamplus.space/register?invite_code=TP000013LEEI"
  }
];
</script>

<style scoped lang="scss">
@use "./index";
</style>
