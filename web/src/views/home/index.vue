<template>
  <div class="home card">
    <!-- 如需恢复原欢迎图首页，取消下一行注释，并可按需注释广告面板 -->
    <!-- <img class="home-bg" src="@/assets/images/welcome.png" alt="welcome" /> -->
    <div class="ad-section">
      <template v-for="(item, index) in adCards" :key="item.title">
        <a :class="['ad-card', `ad-card--${index + 1}`]" :href="item.link" target="_blank" rel="noopener noreferrer">
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
      </template>
    </div>
  </div>
</template>

<script setup lang="ts" name="home">
type AdCard = {
  icon: string;
  tag: string;
  note: string;
  title: string;
  desc: string;
  price: string;
  points: string[];
  cta: string;
  link: string;
};

const adCards: AdCard[] = [
  {
    icon: "Y",
    tag: "网络加速",
    note: "邀请推荐",
    title: "良心云机场推荐",
    desc: "超低月费，适合日常访问、模型使用和稳定开发场景。",
    price: "2 元 / 月 · 100G",
    points: ["流量 100G，满足轻量到中度日常使用。", "不限速，访问和调用体验更稳定。", "设备数无限制，多设备使用更方便。"],
    cta: "前往注册",
    link: "https://xn--9kqz23b19z.com/#/register?code=8QreqkoR"
  },
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
    desc: "无需梯子，直接访问你的 AI 中转站。",
    price: "支持 Codex 中转 · 直达官网",
    points: ["点击后直接跳转到你部署好的站点。", "保留首页广告入口，访问路径更明确。", "适合长期作为首页推荐入口。"],
    cta: "前往站点",
    link: "https://ai.52moyu.net"
  },
  {
    icon: "C",
    tag: "云服务器",
    note: "邀请推荐",
    title: "腾讯云轻量服务器推荐",
    desc: "适合部署后台管理系统、Web 服务和轻量业务应用。",
    price: "低门槛上云 · 适合个人与小团队",
    points: ["适合部署 .NET API 与前端静态资源。", "配置简单，适合快速上线项目。", "对外包、私活和商用项目都比较友好。"],
    cta: "查看方案",
    link: "https://curl.qcloud.com/Uw9jZRJr"
  },
  {
    icon: "T",
    tag: "云产品",
    note: "邀请推荐",
    title: "腾讯云云产品优惠推荐",
    desc: "云服务器、云数据库、COS、CDN、短信等云产品特惠热卖中。",
    price: "项目配套常用云资源 · 一站式选购",
    points: ["适合项目上线后的存储、加速与消息能力补齐。", "云服务器、数据库、对象存储等资源覆盖更全。", "适合商用项目、外包交付和长期运维场景。"],
    cta: "查看优惠",
    link: "https://curl.qcloud.com/Im4ILEeM"
  }
];
</script>

<style scoped lang="scss">
@use "./index";
</style>
