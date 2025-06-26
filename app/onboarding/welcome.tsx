// app/onboarding/welcome.tsx
// Welcome onboarding screen - Modernized

import { router } from 'expo-router';
import React, { useEffect, useRef } from 'react';
import {
  Animated,
  Dimensions,
  SafeAreaView,
  ScrollView,
  StatusBar,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';
import { usePalette } from '../../hooks';
import { useAppDispatch } from '../../store/hooks';

const { width: SCREEN_WIDTH, height: SCREEN_HEIGHT } = Dimensions.get('window');

export default function WelcomeScreen() {
  const colors = usePalette();
  const dispatch = useAppDispatch();

  // Enhanced animations
  const fadeInAnimation = useRef(new Animated.Value(0)).current;
  const slideUpAnimation = useRef(new Animated.Value(50)).current;
  const logoScaleAnimation = useRef(new Animated.Value(0.8)).current;
  const floatingElements = useRef([
    new Animated.Value(0),
    new Animated.Value(0),
    new Animated.Value(0)
  ]).current;

  useEffect(() => {
    // Staggered entrance animations
    Animated.sequence([
      Animated.timing(fadeInAnimation, {
        toValue: 1,
        duration: 800,
        useNativeDriver: true,
      }),
      Animated.parallel([
        Animated.spring(logoScaleAnimation, {
          toValue: 1,
          tension: 50,
          friction: 8,
          useNativeDriver: true,
        }),
        Animated.timing(slideUpAnimation, {
          toValue: 0,
          duration: 600,
          useNativeDriver: true,
        }),
      ])
    ]).start();

    // Floating background elements
    floatingElements.forEach((anim, index) => {
      Animated.loop(
        Animated.sequence([
          Animated.timing(anim, {
            toValue: 1,
            duration: 3000 + (index * 500),
            useNativeDriver: true,
          }),
          Animated.timing(anim, {
            toValue: 0,
            duration: 3000 + (index * 500),
            useNativeDriver: true,
          }),
        ])
      ).start();
    });
  }, []);

  const handleGetStarted = () => {
    router.push('/select-bank');
  };

  return (
    <SafeAreaView style={[styles.container, { backgroundColor: colors.background }]}>
      <StatusBar barStyle="light-content" backgroundColor={colors.background} />
      
      {/* Modern background elements */}
      <View style={styles.backgroundElements}>
        {floatingElements.map((anim, index) => {
          const translateY = anim.interpolate({
            inputRange: [0, 1],
            outputRange: [0, -20],
          });
          
          return (
            <Animated.View 
              key={index}
              style={[
                styles.floatingElement,
                {
                  transform: [{ translateY }],
                  left: `${20 + (index * 30)}%`,
                  top: `${10 + (index * 25)}%`,
                  backgroundColor: index === 0 ? colors.positive : 
                                 index === 1 ? colors.info : colors.warning,
                  opacity: 0.1,
                }
              ]} 
            />
          );
        })}
      </View>

      <Animated.View 
        style={[
          styles.contentWrapper,
          {
            opacity: fadeInAnimation,
            transform: [{ translateY: slideUpAnimation }],
          }
        ]}
      >
        <ScrollView 
          style={styles.scrollView}
          contentContainerStyle={styles.scrollContent}
          showsVerticalScrollIndicator={false}
        >
          {/* Modern App Icon */}
          <View style={styles.heroSection}>
            <Animated.View 
              style={[
                styles.modernAppIcon,
                {
                  backgroundColor: colors.primary,
                  transform: [{ scale: logoScaleAnimation }],
                }
              ]}
            >
              <View style={[styles.iconInner, { backgroundColor: colors.background }]}>
                <Text style={[styles.dollarSign, { color: colors.primary }]}>$</Text>
              </View>
              <View style={[styles.modernBadge, { backgroundColor: colors.positive }]}>
                <Text style={styles.badgeIcon}>📈</Text>
              </View>
            </Animated.View>

            {/* Hero Text */}
            <View style={styles.heroText}>
              <Text style={[styles.modernTitle, { color: colors.text }]}>
                NetWorth
              </Text>
              <Text style={[styles.modernSubtitle, { color: colors.textSecondary }]}>
                Financial insights at your fingertips
              </Text>
              <View style={[styles.tagline, { backgroundColor: colors.backgroundAlt }]}>
                <Text style={[styles.taglineText, { color: colors.primary }]}>
                  💼 Smart • Secure • Simple
                </Text>
              </View>
            </View>
          </View>

          {/* Modern Features Grid */}
          <View style={styles.featuresSection}>
            <Text style={[styles.sectionTitle, { color: colors.text }]}>
              Why NetWorth?
            </Text>
            
            <View style={styles.featuresGrid}>
              <FeatureCard 
                icon="📊" 
                title="Real-time Tracking"
                description="Live updates from your accounts"
                colors={colors} 
              />
              <FeatureCard 
                icon="🤖" 
                title="Smart Categories"
                description="AI-powered expense sorting"
                colors={colors} 
              />
              <FeatureCard 
                icon="📈" 
                title="Net Worth Growth"
                description="Track your financial journey"
                colors={colors} 
              />
              <FeatureCard 
                icon="💡" 
                title="Smart Insights"
                description="Personalized recommendations"
                colors={colors} 
              />
            </View>
          </View>
        </ScrollView>

        {/* Modern CTA Button */}
        <View style={styles.ctaSection}>
          <TouchableOpacity 
            style={[styles.modernButton, { backgroundColor: colors.primary }]}
            onPress={handleGetStarted}
            activeOpacity={0.8}
          >
            <View style={styles.buttonContent}>
              <Text style={[styles.buttonText, { color: colors.background }]}>
                Get Started
              </Text>
              <Text style={[styles.buttonSubtext, { color: colors.background }]}>
                Connect your first account
              </Text>
            </View>
            <View style={[styles.buttonIcon, { backgroundColor: colors.background }]}>
              <Text style={[styles.arrowIcon, { color: colors.primary }]}>→</Text>
            </View>
          </TouchableOpacity>
          
          <View style={styles.securitySection}>
              <View style={styles.securityBadges}>
                <SecurityBadge 
                  icon="🔒" 
                  text="256-bit Encryption" 
                  colors={colors}
                />
                <SecurityBadge 
                  icon="�️" 
                  text="Read-only Access" 
                  colors={colors}
                />
              </View>
            </View>
        </View>
      </Animated.View>
    </SafeAreaView>
  );
}

// Modern Feature Card Component
function FeatureCard({ 
  icon, 
  title, 
  description, 
  colors 
}: { 
  icon: string; 
  title: string; 
  description: string;
  colors: any; 
}) {
  return (
    <View style={[styles.featureCard, { backgroundColor: colors.card }]}>
      <Text style={styles.featureCardIcon}>{icon}</Text>
      <Text style={[styles.featureCardTitle, { color: colors.text }]}>{title}</Text>
      <Text style={[styles.featureCardDescription, { color: colors.textSecondary }]}>
        {description}
      </Text>
    </View>
  );
}

// Modern Security Badge Component
function SecurityBadge({ 
  icon, 
  text, 
  colors 
}: { 
  icon: string; 
  text: string; 
  colors: any; 
}) {
  return (
    <View style={[styles.securityBadge, { 
      backgroundColor: colors.backgroundAlt,
      borderColor: colors.border 
    }]}>
      <Text style={styles.securityBadgeIcon}>{icon}</Text>
      <Text style={[styles.securityBadgeText, { color: colors.textSecondary }]}>
        {text}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  backgroundElements: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    overflow: 'hidden',
  },
  floatingElement: {
    position: 'absolute',
    width: 60,
    height: 60,
    borderRadius: 30,
  },
  contentWrapper: {
    flex: 1,
  },
  scrollView: {
    flex: 1,
  },
  scrollContent: {
    paddingBottom: 20,
  },
  heroSection: {
    alignItems: 'center',
    paddingTop: 60,
    paddingHorizontal: 24,
    marginBottom: 40,
  },
  modernAppIcon: {
    width: 100,
    height: 100,
    borderRadius: 28,
    alignItems: 'center',
    justifyContent: 'center',
    position: 'relative',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 8 },
    shadowOpacity: 0.3,
    shadowRadius: 16,
    elevation: 16,
    marginBottom: 24,
  },
  iconInner: {
    width: 60,
    height: 60,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center',
  },
  dollarSign: {
    fontSize: 32,
    fontWeight: 'bold',
  },
  modernBadge: {
    position: 'absolute',
    top: -8,
    right: -8,
    width: 32,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
    elevation: 4,
  },
  badgeIcon: {
    fontSize: 16,
  },
  heroText: {
    alignItems: 'center',
  },
  modernTitle: {
    fontSize: 36,
    fontWeight: '800',
    textAlign: 'center',
    marginBottom: 8,
    letterSpacing: -1,
  },
  modernSubtitle: {
    fontSize: 18,
    textAlign: 'center',
    marginBottom: 16,
    fontWeight: '400',
  },
  tagline: {
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 20,
  },
  taglineText: {
    fontSize: 14,
    fontWeight: '600',
  },
  featuresSection: {
    paddingHorizontal: 24,
    marginBottom: 40,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '700',
    marginBottom: 24,
    textAlign: 'center',
  },
  featuresGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: 32,
  },
  featureCard: {
    width: (SCREEN_WIDTH - 60) / 2,
    padding: 20,
    borderRadius: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 8,
    elevation: 4,
  },
  featureCardIcon: {
    fontSize: 32,
    marginBottom: 12,
  },
  featureCardTitle: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 6,
  },
  featureCardDescription: {
    fontSize: 13,
    lineHeight: 18,
  },
  securitySection: {
    alignItems: 'center',
  },
  securityTitle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 16,
    textAlign: 'center',
  },
  securityBadges: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center',
    gap: 8,
  },
  securityBadge: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 12,
    paddingVertical: 8,
    borderRadius: 20,
    borderWidth: 1,
    marginHorizontal: 4,
    marginVertical: 4,
  },
  securityBadgeIcon: {
    fontSize: 12,
    marginRight: 6,
  },
  securityBadgeText: {
    fontSize: 12,
    fontWeight: '500',
  },
  ctaSection: {
    paddingHorizontal: 24,
    paddingBottom: 40,
    paddingTop: 20,
  },
  modernButton: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 20,
    paddingHorizontal: 24,
    borderRadius: 20,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 6 },
    shadowOpacity: 0.2,
    shadowRadius: 12,
    elevation: 12,
    marginBottom: 16,
  },
  buttonContent: {
    flex: 1,
  },
  buttonText: {
    fontSize: 20,
    fontWeight: '700',
    marginBottom: 4,
  },
  buttonSubtext: {
    fontSize: 14,
    opacity: 0.8,
  },
  buttonIcon: {
    width: 40,
    height: 40,
    borderRadius: 20,
    alignItems: 'center',
    justifyContent: 'center',
  },
  arrowIcon: {
    fontSize: 18,
    fontWeight: 'bold',
  },
  disclaimer: {
    fontSize: 12,
    textAlign: 'center',
    lineHeight: 16,
  },
});
